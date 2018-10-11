
public interface ISystemInterface 
{
	// Use this for initialization
	void Start(World world);
	
	// Update is called once per frame
	void Update(World world, float time = 0f, float deltaTime = 0f);
}
