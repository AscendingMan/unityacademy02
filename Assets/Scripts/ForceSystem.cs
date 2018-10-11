using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSystem : ISystemInterface 
{
    public void Start(World world)
    {
        var entities = world.entities;
        
        // add randomized velocity to all entities that have positions
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new Entity(i), EntityFlags.kFlagForce);

                var forceComponent = new ForceComponent() {massInverse = Random.Range(1f, 5f), force = Vector2.zero};
                entities.forceComponents[i] = forceComponent;
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition) && 
                entities.flags[i].HasFlag(EntityFlags.kFlagForce) &&
                entities.flags[i].HasFlag(EntityFlags.kFlagMove))
            {
                var moveComponent = entities.moveComponents[i];
                var forceComponent = entities.forceComponents[i];

                // F = m * a => a = F / m
                moveComponent.acceleration = forceComponent.massInverse * forceComponent.force;
                forceComponent.force = Vector2.zero;

                entities.moveComponents[i] = moveComponent;
                entities.forceComponents[i] = forceComponent;
            }
        }
    }
}
