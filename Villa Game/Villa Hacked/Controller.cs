public abstract class Controller<T> : BaseController where T : Controller<T>
{
	public static T Instance { get; private set; }

	public override void Preload()
	{
		Instance = (T)this;
	}

	protected virtual void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
