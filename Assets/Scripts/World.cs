using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

// Entry point to the simulations. Technically there could be multiple worlds
// that are be completely isolated from each other
public class World : MonoBehaviour 
{	
	public GameObject templateObject;
	public int entityCount = 10;
	public Rect worldBounds = new Rect(-10f, -5f, 20f, 10f);
	public Vector2 gravity = Vector2.down * 9.81f;

	[NonSerialized]
	public Entities entities;

	protected List<ISystemInterface> systems;
	
	// Use this for initialization
	void Start () 
	{
		Profiler.BeginSample("World.Start");
		systems = new List<ISystemInterface>();
		entities = new Entities();
		
		entities.Init(entityCount);

		// System addition order matters, they will run in the same order
		systems.Add(new GravitySystem());
		systems.Add(new ForceSystem());
		systems.Add(new MoveSystem());	
		systems.Add(new CollisionSystem());
		systems.Add(new WorldBoundsSystem());
		systems.Add(new RenderingSystem());

		foreach (var system in systems)
		{
			Profiler.BeginSample(system.GetType().Name);
			system.Start(this);
			Profiler.EndSample();
		}
		
		Profiler.EndSample();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Profiler.BeginSample("World.Update");
		foreach (var system in systems)
		{
			Profiler.BeginSample(system.GetType().Name);
			system.Update(this, Time.timeSinceLevelLoad, Time.deltaTime);
			Profiler.EndSample();
		}
		Profiler.EndSample();
	}
}
