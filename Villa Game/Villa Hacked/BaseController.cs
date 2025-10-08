using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
	public virtual int PreloadOrder => 0;

	public virtual int InitOrder => 0;

	public virtual void Preload()
	{
	}

	public virtual void Init()
	{
	}
}
