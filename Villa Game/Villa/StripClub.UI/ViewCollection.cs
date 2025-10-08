using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StripClub.UI;

public abstract class ViewCollection<TView> : MonoBehaviour, IViewManager where TView : IView
{
	[SerializeField]
	protected readonly List<TView> views = new List<TView>();

	public IEnumerable<TView> VisibleViews => views.Where((TView _view) => _view.IsActive());

	public virtual void HideAll()
	{
		foreach (TView item in views.Where((TView _view) => _view.IsActive()))
		{
			item.Display(isOn: false);
		}
	}
}
