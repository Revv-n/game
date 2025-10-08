using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Lootboxes;
using GreenT.UI;
using JetBrains.Annotations;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class LootboxContentView : LootboxContentBaseView
{
	[SerializeField]
	private FlexibleGridLayoutGroup _smallCardLayoutGroup;

	private bool showBigCards;

	private bool showSmallCards;

	private ISmallCardViewStrategy _currentViewStrategy;

	private readonly ISmallCardViewStrategy _bigCardStrategy = new SmallCardWithBigCardViewStrategy();

	private readonly ISmallCardViewStrategy _smallCardStrategy = new SmallCardViewStrategy();

	[Inject]
	public void Init(FakeAssetService fakeAssetService)
	{
		_fakeAssetService = fakeAssetService;
	}

	public void SetGuarantedRewardsWithViewSettings(LootboxLinkedContent content, BundleLot.ViewSettings settings)
	{
		SetRewards(content.Lootbox.GuarantedDrop, settings.Focus);
	}

	public void SetPossibleRewardsWithLootboxRarity(LootboxLinkedContent content)
	{
		IEnumerable<DropSettings> drops = content.Lootbox.DropOptions.Cast<DropSettings>();
		SetRewards(drops, null, (int)content.Lootbox.Rarity, isShowingQuantity: false);
	}

	public void Set(LootboxLinkedContent content, DropSettings hidedDrop)
	{
		_bigCardsViewManager.HideAll();
		_smallCardsViewManager.HideAll();
		bool active = false;
		foreach (DropSettings item in content.Lootbox.GuarantedDrop)
		{
			if (item != hidedDrop)
			{
				_smallCardsViewManager.Display(item);
			}
			active = true;
		}
		_smallOptionsContainer.gameObject.SetActive(active);
	}

	private void SetRewards(IEnumerable<DropSettings> drops, [CanBeNull] List<string> focus = null, int? allRewardsForcedRarity = null, bool isShowingQuantity = true)
	{
		HideAll();
		foreach (DropSettings drop in drops)
		{
			if (focus != null && focus.Any() && ((drop.Selector is SelectorByID selectorByID && focus.Contains(selectorByID.ID.ToString())) || (drop.Selector is CurrencySelector currencySelector && focus.Contains(SelectorTools.GetResourceNameValueByType(currencySelector.Currency)))))
			{
				DisplayBigCard(drop);
			}
			else
			{
				DisplaySmallCard(drop, allRewardsForcedRarity, isShowingQuantity ? new int?(drop.Quantity) : null);
			}
		}
		EnableOptionContainers();
		CheckViewStrategy(drops);
	}

	private IView DisplayBigCard(DropSettings drop)
	{
		showBigCards = true;
		return _bigCardsViewManager.Display(drop);
	}

	private IView DisplaySmallCard(DropSettings drop, int? rarity = null, int? quantity = null)
	{
		return DisplaySmallCard(drop.Type, drop.Selector, rarity, quantity);
	}

	private IView DisplaySmallCard(RewType type, Selector selector, int? rarity = null, int? quantity = null)
	{
		showSmallCards = true;
		return _smallCardsViewManager.Display(type, selector, rarity, quantity);
	}

	private void EnableOptionContainers()
	{
		_smallOptionsContainer.gameObject.SetActive(showSmallCards);
		_bigOptionsContainer.gameObject.SetActive(showBigCards);
	}

	private void CheckViewStrategy(IEnumerable<DropSettings> drops)
	{
		if (!(_smallCardLayoutGroup == null))
		{
			_currentViewStrategy = (showBigCards ? _bigCardStrategy : _smallCardStrategy);
			_currentViewStrategy.UpdateConstraint(_smallCardLayoutGroup, drops.Count());
		}
	}
}
