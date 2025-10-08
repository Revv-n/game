using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace StripClub.UI.Shop;

public class RarityChancesView : MonoView<Dictionary<int, decimal>>
{
	private IViewManager<DropChanceView> dropViewManager;

	[Inject]
	public void Init(IViewManager<DropChanceView> dropViewManager)
	{
		this.dropViewManager = dropViewManager;
	}

	public override void Set(Dictionary<int, decimal> chances)
	{
		base.Set(chances);
		dropViewManager.HideAll();
		foreach (KeyValuePair<int, decimal> item in base.Source.Where((KeyValuePair<int, decimal> pair) => pair.Value > 0m))
		{
			dropViewManager.GetView().Set(item.Key, item.Value);
		}
	}
}
