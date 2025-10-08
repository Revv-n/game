using System.Collections.Generic;
using UnityEngine;

namespace StripClub.UI;

public class NodeMonoView<T> : MonoView<T>, IView<T>, IView
{
	[SerializeField]
	public List<MonoView<T>> innerViews = new List<MonoView<T>>();

	public override void Set(T source)
	{
		base.Set(source);
		foreach (MonoView<T> innerView in innerViews)
		{
			innerView.Set(source);
		}
	}

	public override void Display(bool display)
	{
		base.Display(display);
		foreach (MonoView<T> innerView in innerViews)
		{
			innerView.Display(display);
		}
	}
}
