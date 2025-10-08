using System;
using System.Linq;
using GreenT;
using UnityEngine;

namespace StripClub.UI.Shop;

public class BundleChainLotViewFactory : ContainerViewFactory
{
	public override ContainerView Create(LotContainer lotLotContainer)
	{
		try
		{
			LotView lotView = viewPrefabs.First();
			ContainerView containerView = container.InstantiatePrefabForComponent<ContainerView>((UnityEngine.Object)lotView, viewContainer);
			containerView.Set(lotLotContainer);
			return containerView;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Imposible to create view for LOT with ID: " + lotLotContainer.Lots.ToList()[0].ID);
		}
	}
}
