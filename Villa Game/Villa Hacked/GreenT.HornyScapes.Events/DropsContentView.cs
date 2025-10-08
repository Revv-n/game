using System.Collections.Generic;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model.Shop.UI;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class DropsContentView : MonoView
{
	private SmallCardsViewManager smallViewsManager;

	[Inject]
	public void Init(SmallCardsViewManager smallViewsManager)
	{
		this.smallViewsManager = smallViewsManager;
	}

	public void Set(List<DropSettings> drops)
	{
		smallViewsManager.HideAll();
		foreach (DropSettings drop in drops)
		{
			smallViewsManager.Display(drop);
		}
	}
}
