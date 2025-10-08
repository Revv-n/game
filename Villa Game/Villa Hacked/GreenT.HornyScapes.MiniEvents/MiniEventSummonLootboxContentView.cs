using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.UI;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventSummonLootboxContentView : LootboxContentBaseView
{
	public void Set(SummonLot summonLot)
	{
		HideAll();
		bool isShow = false;
		bool isShow2 = false;
		Lootbox lootbox = (summonLot.SingleRewardSettings.Reward as LootboxLinkedContent).Lootbox;
		List<ICharacter> list = new List<ICharacter>();
		List<RandomDropSettings> list2 = new List<RandomDropSettings>();
		foreach (RandomDropSettings dropOption in lootbox.DropOptions)
		{
			if (dropOption.Type != 0)
			{
				continue;
			}
			Selector selector2 = dropOption.Selector;
			SelectorByID selector = selector2 as SelectorByID;
			if (selector != null)
			{
				ICharacter item = _cards.Collection.OfType<ICharacter>().First((ICharacter x) => x.ID == selector.ID);
				list.Add(item);
				list.OrderBy((ICharacter c) => c.Rarity);
				list2.Insert(list.IndexOf(item), dropOption);
			}
		}
		list2.Sum((RandomDropSettings d) => d.Weight);
		for (int i = 0; i < list.Count; i++)
		{
			if (i < list.Count - 1)
			{
				_smallCardsViewManager.Display(list2[i]);
				isShow2 = true;
				continue;
			}
			_ = list[i].BankImages.Big;
			CardDropView view = _bigCardsViewManager.GetView();
			_fakeAssetService.SetFakeCharacterBankImages(list[i], view.icon, (ICharacter character) => character.BankImages.Big);
			view.SetCharacter(list[i], list2[i].Quantity, (int)list[i].Rarity);
			isShow = true;
		}
		TryShow(_smallOptionsContainer.gameObject, isShow2);
		TryShow(_bigOptionsContainer.gameObject, isShow);
	}
}
