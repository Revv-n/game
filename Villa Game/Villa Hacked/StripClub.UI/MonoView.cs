using UnityEngine;

namespace StripClub.UI;

public abstract class MonoView<T> : MonoView, IView<T>, IView
{
	public T Source { get; private set; }

	public virtual void Set(T source)
	{
		Source = source;
	}
}
public abstract class MonoView : MonoBehaviour, IView
{
	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	public virtual bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public virtual void Display(bool display)
	{
		if (IsActive() != display)
		{
			base.gameObject.SetActive(display);
		}
	}
}
