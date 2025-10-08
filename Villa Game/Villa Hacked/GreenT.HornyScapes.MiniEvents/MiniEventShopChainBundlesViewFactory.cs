using System;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopChainBundlesViewFactory : AbstractLotViewFactory<Lot, ChainBundleLotView>
{
	[SerializeField]
	private ChainBundleLotView _prefab;

	public override ChainBundleLotView Create(Lot lot)
	{
		try
		{
			ChainBundleLotView prefab = _prefab;
			ChainBundleLotView chainBundleLotView = container.InstantiatePrefabForComponent<ChainBundleLotView>((UnityEngine.Object)prefab, viewContainer);
			chainBundleLotView.Set(lot);
			return chainBundleLotView;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Imposible to create ChainBundleLotView for LOT with ID: " + lot.ID);
		}
	}
}
