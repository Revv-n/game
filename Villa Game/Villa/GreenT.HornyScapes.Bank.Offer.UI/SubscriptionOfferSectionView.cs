using System.Collections.Generic;
using GreenT.HornyScapes.Subscription.Push;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SubscriptionOfferSectionView : AbstractLotSectionView<SubscriptionPushSettings>
{
	[SerializeField]
	private LocalizedTextMeshPro _pushHeader;

	[SerializeField]
	private LocalizedTextMeshPro _pushHeaderShadow;

	[SerializeField]
	private WindowOpener _windowOpener;

	public override void Set(SubscriptionPushSettings settings)
	{
		base.Set(settings);
		_pushHeader.Init(settings.LocalizationKey);
		_pushHeaderShadow.Init(settings.LocalizationKey);
	}

	protected override IEnumerable<Lot> TargetLots(IEnumerable<Lot> collection)
	{
		return base.Source.SubscriptionID;
	}

	protected void SetGoToInfo()
	{
		foreach (ContainerView visibleView in viewManager.VisibleViews)
		{
			foreach (SubscriptionLotView lotView in visibleView.LotViews)
			{
				lotView.SetGoTo();
				lotView.OnGoToClick.Take(1).Subscribe(CloseWindowAfterGoToBank);
			}
		}
	}

	private void CloseWindowAfterGoToBank(Unit unit)
	{
		_windowOpener.Close();
	}
}
