using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopChainBundlesSingleRootView : MonoView<IEnumerable<Lot>>
{
	[SerializeField]
	private Transform _backgroundHolder;

	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private LocalizedTextMeshPro _description;

	private ChainBundlesSectionBackground _chainBundlesSectionBackground;

	private MiniEventShopChainBundlesViewManager _miniEventShopChainBundlesViewManager;

	private const int MIN_AMOUNT = 1;

	private const string TITLE_KEY = "ui.shop.bundles.chain.{0}.title";

	private const string DESCRIPTION_KEY = "ui.shop.bundles.chain.{0}.description";

	[Inject]
	private void Init(MiniEventShopChainBundlesViewManager miniEventShopChainBundlesViewManager)
	{
		_miniEventShopChainBundlesViewManager = miniEventShopChainBundlesViewManager;
	}

	public override void Set(IEnumerable<Lot> sources)
	{
		base.Set(sources);
		DisplayAvailableLots(sources);
	}

	private void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		try
		{
			_miniEventShopChainBundlesViewManager.HideAll();
			if (!lots.Any())
			{
				return;
			}
			GetBackground(lots.First());
			SetupLocalization();
			lots = lots.OrderByDescending((Lot lot) => lot.SerialNumber);
			List<ChainBundleLotView> list = new List<ChainBundleLotView>();
			foreach (Lot lot in lots)
			{
				if (lot.Received < lot.AvailableCount)
				{
					ChainBundleLotView item = _miniEventShopChainBundlesViewManager.Display(lot as BundleLot);
					list.Add(item);
				}
			}
			if (list.Any())
			{
				list.Reverse();
				UpdateBundleChainStates(list);
				SetupBundleChainBackgroundView(list);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Impossible to display CHAIN BUNDLES with ID: " + ((base.Source != null && base.Source.Any()) ? base.Source.First().ID : 0));
		}
	}

	private void SetupLocalization()
	{
		string key = $"ui.shop.bundles.chain.{base.Source.First().TabID}.title";
		string key2 = $"ui.shop.bundles.chain.{base.Source.First().TabID}.description";
		_title.Init(key);
		_titleShadow.Init(key);
		_description.Init(key2);
	}

	private void UpdateBundleChainStates(List<ChainBundleLotView> chainBundleViews)
	{
		bool flag = true;
		ChainBundleLotView chainBundleLotView = chainBundleViews.First();
		for (int i = 1; i < chainBundleViews.Count; i++)
		{
			if (flag && chainBundleLotView.Source.IsAvailable())
			{
				chainBundleLotView.SetupAvailabilityState(flag);
				flag = false;
			}
			chainBundleLotView = chainBundleViews[i];
			chainBundleLotView.SetupAvailabilityState(flag);
		}
		if (chainBundleViews.Count() == 1)
		{
			chainBundleLotView.SetupAvailabilityState(flag);
		}
	}

	private void GetBackground(Lot lot)
	{
		if (_chainBundlesSectionBackground != null)
		{
			UnityEngine.Object.Destroy(_chainBundlesSectionBackground.gameObject);
		}
		GameObject background = (lot as BundleLot).Settings.GetBackground();
		_ = background == null;
		background = UnityEngine.Object.Instantiate(background, _backgroundHolder);
		_chainBundlesSectionBackground = background.GetComponent<ChainBundlesSectionBackground>();
		_ = _chainBundlesSectionBackground == null;
	}

	private void SetupBundleChainBackgroundView(List<ChainBundleLotView> chainBundleViews)
	{
		Sprite tabBackground = _chainBundlesSectionBackground.TabBackground;
		Sprite tabArrow = _chainBundlesSectionBackground.TabArrow;
		for (int i = 0; i < chainBundleViews.Count; i++)
		{
			chainBundleViews[i].SetupBackground(tabBackground, tabArrow);
			chainBundleViews[i].SetupArrow(i + 1 >= chainBundleViews.Count);
		}
	}
}
