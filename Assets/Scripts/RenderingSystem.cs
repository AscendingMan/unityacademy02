using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingSystem : ISystemInterface
{
	private const int BATCH_SIZE = 1000;
	
	public void Start(World world)
	{
		//do nothing
	}

	public void Update(World world, float time = 0, float deltaTime = 0)
	{
		var entities = world.entities;
		Mesh mesh = world.templateObject.GetComponent<MeshFilter>().sharedMesh;
		Material material = world.templateObject.GetComponent<Renderer>().sharedMaterial;
	
		List<Matrix4x4> transformList = new List<Matrix4x4>();

		for (var i = 0; i < entities.flags.Count; i++)
		{
			var pos = entities.positions[i];
			var mtrx = new Matrix4x4();
			var scale = 2.0f * Vector2.one;

			if (entities.flags[i].HasFlag(EntityFlags.kFlagCollision))
				scale *= entities.collisionComponents[i].radius;
		
			mtrx.SetTRS(pos, Quaternion.Euler(Vector3.zero), scale);
		
			transformList.Add(mtrx);

			// DrawMeshInstanced has limitation of up to 1023(?) items per single call
			if (transformList.Count >= BATCH_SIZE)
			{
				Graphics.DrawMeshInstanced(mesh, 0, material, transformList);
				transformList.Clear();
			}
		}	
		
		// Remaining objects
		if (transformList.Count > 0)
			Graphics.DrawMeshInstanced(mesh, 0, material, transformList);
	}
}
