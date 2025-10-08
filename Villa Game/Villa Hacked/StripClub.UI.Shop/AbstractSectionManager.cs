using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public abstract class AbstractSectionManager<TKey, TSection, TView> : MonoBehaviour where TSection : IBankSection where TView : IView<TSection>
{
	protected readonly List<TView> views = new List<TView>();

	protected IFactory<TKey, TView> sectionFactory;

	[Inject]
	public void Init(IFactory<TKey, TView> sectionFactory)
	{
		this.sectionFactory = sectionFactory;
	}

	public TView GetView(TKey key)
	{
		HideAll();
		TView val = SelectViewByKey(key);
		if (val == null)
		{
			val = ((IFactory<TKey, _003F>)(object)sectionFactory).Create(key);
			views.Add(val);
		}
		else
		{
			val.Display(isOn: true);
		}
		return val;
	}

	protected abstract TView SelectViewByKey(TKey key);

	public void HideAll()
	{
		foreach (TView item in views.Where((TView _view) => _view.IsActive()))
		{
			item.Display(isOn: false);
		}
	}
}
