Original readme below:
# Mini Physics Engine

Sample C# project for an internal Unity lecture in 2018, aimed at junior/future engineers.

This is a super-simple C# "game" + "physics engine" _(just a bunch of sprites moving around with very small amount of logic really)_. I made to
show how one might do simple 2D physics engine from scratch.

The example project should work on Unity 2018.2.x and later.

[Presentation slides](https://drive.google.com/open?id=14LapDFZiQ_3oiyy2_s6-N4lyIYfNeTam)

I used some resources to make life easier for me here:

* Sprites used by "the game" I took from Dan Cook's [Space Cute prototyping challenge](http://www.lostgarden.com/2007/03/spacecute-prototyping-challenge.html).
* It is somewhat successor to [Aras presentation on DoD](https://github.com/aras-p/dod-playground).
* Circle collision impulse resolution was inspired by great [Randy Gaul's post](https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331).

---
Appended readme starts here
# An attempt to implement AABB collisions in the given mini-physics engine

First, I added separate edge lenghts for x and y to entities
```cpp
public struct CollisionComponent
{
    public float yLength;
    public float xLength;
	public float coeffOfRestitution;
    public float radius; //obsolete
}
```

Then, appended this to WorldBoundsSystem
```cpp
// Apply only if object is leaving horizontal boundaries
if ((pos.x - (xLength/2) < bounds.xMin) && (moveComponent.velocity.x < 0f) || 
	(pos.x + (xLength/2) > bounds.xMax) && (moveComponent.velocity.x > 0f))
{
	moveComponent.velocity.x = -coeffOfRestitution * moveComponent.velocity.x;
	pos.x = Mathf.Clamp(pos.x, bounds.xMin + radius, bounds.xMax + radius);
}

// Apply only if object is leaving vertical boundaries
if ((pos.y - (yLength/2) < bounds.yMin) && (moveComponent.velocity.y < 0f) || 
	(pos.y + (yLength/2) > bounds.yMax) && (moveComponent.velocity.y > 0f))
{
	moveComponent.velocity.y = -coeffOfRestitution * moveComponent.velocity.y;
	pos.y = Mathf.Clamp(pos.y, bounds.yMin + radius, bounds.yMax - radius);
}
```

Started the actual collision work by implementing the most dirty version of AABB check
```cpp
bool TestAABBAABB(Entities e, int index1, int index2)
{
	if (e.positions[index1].x + (e.collisionComponents[index1].xLength + .1f) <
		e.positions[index2].x - (e.collisionComponents[index2].xLength - .1f) ||
		e.positions[index2].x + (e.collisionComponents[index2].xLength + .1f) <
		e.positions[index1].x - (e.collisionComponents[index1].xLength - .1f))
	{
		return false;
	}
	if (e.positions[index1].y + (e.collisionComponents[index1].yLength + .1f) <
		e.positions[index2].y - (e.collisionComponents[index2].yLength - .1f) ||
		e.positions[index2].y + (e.collisionComponents[index2].yLength + .1f) <
		e.positions[index1].y - (e.collisionComponents[index1].yLength - .1f))
	{
		return false;
	}

	Debug.Log("Collision");

	return true;
}
```

Aaaaaand that's where it kinda breaks since I didn't know about tunneling and dynamic intersection tests. I put down a function IntersectMovingAABBAABB (taken from Christer Ericson's Real-Time Collision Detection, as noted in the code as well) for starters, but didn't get much anywhere from there on. There should have been some sort of continuous collision detection to have the project actually work as intended, but ehh.
