using System;
using System.Linq;
using GreenT;

namespace StripClub.UI.Shop;

public class BundleChainLotViewFactory : ContainerViewFactory
{
	public override ContainerView Create(LotContainer lotLotContainer)
	{
		try
		{
			LotView prefab = viewPrefabs.First();
			ContainerView containerView = container.InstantiatePrefabForComponent<ContainerView>(prefab, viewContainer);
			containerView.Set(lotLotContainer);
			return containerView;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Imposible to create view for LOT with ID: " + lotLotContainer.Lots.ToList()[0].ID);
		}
	}
}
