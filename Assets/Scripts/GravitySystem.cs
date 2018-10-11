using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem : ISystemInterface 
{
    public void Start(World world)
    {
        var entities = world.entities;
        
        // add randomized velocity to all entities that have positions
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new Entity(i), EntityFlags.kFlagGravity);
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var gravity = world.gravity;
        
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagGravity) && 
                entities.flags[i].HasFlag(EntityFlags.kFlagForce))
            {
                var forceComponent = entities.forceComponents[i];
                
                // F = m * g
                if (forceComponent.massInverse > 1e-6f)
                    forceComponent.force += gravity / forceComponent.massInverse;
                
                entities.forceComponents[i] = forceComponent;
            }
        }
    }
}
