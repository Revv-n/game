using System.Collections.Generic;
using GreenT.HornyScapes.DebugInfo;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferSectionView : AbstractLotSectionView<OfferSettings>
{
	[SerializeField]
	private DebugInfoContainer _debugInfo;

	protected override IEnumerable<Lot> TargetLots(IEnumerable<Lot> collection)
	{
		TryDebug(base.Source);
		return base.Source.Bundles;
	}

	private void TryDebug(OfferSettings source)
	{
	}
}
