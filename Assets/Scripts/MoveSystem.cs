using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSystem : ISystemInterface 
{
    public void Start(World world)
    {
        var entities = world.entities;
        
        // add randomized velocity to all entities that have positions
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new Entity(i), EntityFlags.kFlagMove);
                
                var moveComponent = entities.moveComponents[i];
                moveComponent.velocity = new Vector2(Random.Range(-3f,3f), Random.Range(-3f, 3f));
                entities.moveComponents[i] = moveComponent;
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagMove))
            {
                // pos = pos + v * dt + a * t^2 / 2
                entities.positions[i] += entities.moveComponents[i].velocity * deltaTime +
                                         0.5f * entities.moveComponents[i].acceleration * deltaTime * deltaTime;

                var moveComponent = entities.moveComponents[i];
                moveComponent.velocity += entities.moveComponents[i].acceleration * deltaTime;
                entities.moveComponents[i] = moveComponent;
            }
        }
    }
}
