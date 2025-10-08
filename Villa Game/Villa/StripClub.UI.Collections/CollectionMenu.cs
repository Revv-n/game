using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Collections;

public class CollectionMenu : MonoBehaviour
{
	private IList<Tab> tabs;

	private IPlayerBasics playerBasics;

	private ISaver saver;

	private CardsCollection cards;

	private TabView.Manager tabViewManager;

	[Inject]
	public void Init(IList<Tab> tabs, IPlayerBasics playerBasics, ISaver saver, CardsCollection cards, TabView.Manager tabViewManager)
	{
		this.tabs = tabs;
		this.playerBasics = playerBasics;
		this.saver = saver;
		this.cards = cards;
		this.tabViewManager = tabViewManager;
	}

	private void OnEnable()
	{
		InitTabs();
	}

	public void InitTabs()
	{
		tabViewManager.HideAll();
		foreach (Tab item in tabs.Where(IsTabUnlocked))
		{
			TabView view = tabViewManager.GetView();
			view.Init(item);
			saver.Add(view);
		}
	}

	private bool IsTabUnlocked(Tab tab)
	{
		return cards.Collection.All((ICard _card) => _card.GroupID != tab.ID);
	}
}
