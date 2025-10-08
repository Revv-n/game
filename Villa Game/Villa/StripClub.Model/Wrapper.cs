namespace StripClub.Model;

public class Wrapper<T>
{
	public T Instance { get; private set; }

	public Wrapper()
	{
	}

	public Wrapper(T obj)
	{
		Instance = obj;
	}

	public void Set(T newObj)
	{
		Instance = newObj;
	}
}
