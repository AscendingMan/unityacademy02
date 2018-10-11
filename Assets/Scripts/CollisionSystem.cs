using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystemInterface
{
    private Vector2[] velocityCache;
    
    public void Start(World world)
    {
        var entities = world.entities;

        velocityCache = new Vector2[entities.flags.Count];
        
        // add randomized collision radius (derived from mass) and coefficient of restitution
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition) &&
                entities.flags[i].HasFlag(EntityFlags.kFlagForce))
            {
                entities.AddComponent(new Entity(i), EntityFlags.kFlagCollision);
                var collisionComponent = new CollisionComponent();

                if (entities.forceComponents[i].massInverse > 1e-6f)
                    collisionComponent.radius = 1.0f / entities.forceComponents[i].massInverse;

                collisionComponent.coeffOfRestitution = Random.Range(0.1f, 0.9f);

                entities.collisionComponents[i] = collisionComponent;
            }
        }
    }

    public static bool CirclesCollide(Vector2 pos1, float r1, Vector2 pos2, float r2)
    {
        // |pos1 - pos2| <= |r1+r2| is the same as
        // (pos1 - pos2)^2 <= (r1+r2)^2
        return (pos2 - pos1).sqrMagnitude <= (r2 + r1) * (r2 + r1);
    }

    // Impulse resolution inspired by:
    // https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        
        // Init velocity cache
        for (var i = 0; i < entities.flags.Count; i++)
        {            
            velocityCache[i] = entities.moveComponents[i].velocity;
        }
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            // Check all pairs only once
            for (var j = i + 1; j < entities.flags.Count; j++)
            {
                if (entities.flags[i].HasFlag(EntityFlags.kFlagCollision) && 
                    entities.flags[j].HasFlag(EntityFlags.kFlagCollision))
                {
                    var col1 = entities.collisionComponents[i];
                    var col2 = entities.collisionComponents[j];

                    var pos1 = entities.positions[i];
                    var pos2 = entities.positions[j];

                    if (CirclesCollide(pos1, col1.radius, pos2, col2.radius))
                    {
                        var move1 = entities.moveComponents[i];
                        var move2 = entities.moveComponents[j];

                        // Relative velocity
                        Vector2 relVel = move2.velocity - move1.velocity;
                        // Collision normal
                        Vector2 normal = (pos2 - pos1).normalized;

                        float velocityProjection = Vector2.Dot(relVel, normal);

                        // Process only if objects are not separating
                        if (velocityProjection < 0)
                        {
                            var force1 = entities.forceComponents[i];
                            var force2 = entities.forceComponents[j];
                            
                            float cr = Mathf.Min(col1.coeffOfRestitution, col2.coeffOfRestitution);
                            
                            // Impulse scale
                            float impScale = -(1f + cr) * velocityProjection /
                                             (force1.massInverse + force2.massInverse);

                            Vector2 impulse = impScale * normal;

                            velocityCache[i] -= force1.massInverse * impulse;
                            velocityCache[j] += force2.massInverse * impulse;
                        }        
                    }
                }
            }
            
        }

        // Apply cached velocities
        for (var i = 0; i < entities.flags.Count; i++)
        {
            var move1 = entities.moveComponents[i];
            move1.velocity = velocityCache[i];
            entities.moveComponents[i] = move1;
            
            velocityCache[i] = Vector2.zero;
        }
    }
}
