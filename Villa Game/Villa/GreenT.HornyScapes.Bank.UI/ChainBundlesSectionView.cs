using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.UI;

public sealed class ChainBundlesSectionView : BankSectionView
{
	[SerializeField]
	private Transform _backgroundHolder;

	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	private ChainBundlesSectionBackground _chainBundlesSectionBackground;

	private const int MIN_AMOUNT = 1;

	private const string TITLE_KEY = "ui.shop.bundles.chain.{0}.title";

	protected override void DisplayAvailableLots(IEnumerable<Lot> lots)
	{
		try
		{
			viewManager.HideAll();
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
					LotContainer source = new LotContainer(new Lot[1] { lot });
					ContainerView containerView = viewManager.Display(source);
					list.Add(containerView.LotViews.First() as ChainBundleLotView);
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
			ExceptionReport(lots);
			throw innerException.SendException("Impossible to display section with ID: " + base.Source.ID);
		}
	}

	protected override IEnumerable<Lot> VisibleLots(IEnumerable<Lot> lots)
	{
		return lots;
	}

	private void SetupLocalization()
	{
		string key = $"ui.shop.bundles.chain.{base.Source.ID}.title";
		_title.Init(key);
		_titleShadow.Init(key);
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
