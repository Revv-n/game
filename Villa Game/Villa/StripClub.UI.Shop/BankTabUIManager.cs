using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class BankTabUIManager : MonoBehaviour
{
	private IFactory<IView<BankTab>> viewFactory;

	private readonly List<IView<BankTab>> views = new List<IView<BankTab>>();

	[Inject]
	public void Init(IFactory<IView<BankTab>> viewFactory)
	{
		this.viewFactory = viewFactory;
	}

	public IView<BankTab> GetView(BankTab tab)
	{
		IView<BankTab> view = views.FirstOrDefault((IView<BankTab> _tab) => !_tab.IsActive());
		if (view == null)
		{
			view = viewFactory.Create();
			views.Add(view);
		}
		else
		{
			view.Display(isOn: true);
		}
		view.Set(tab);
		return view;
	}

	public void HideAll()
	{
		foreach (IView<BankTab> view in views)
		{
			view.Display(isOn: false);
		}
	}
}
