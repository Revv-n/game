using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class LotViewFactory : MonoBehaviour, IFactory<Lot, LotView>, IFactory
{
	protected Dictionary<Type, Type> typeCorrespondancyDict = new Dictionary<Type, Type>
	{
		{
			typeof(GemShopLot),
			typeof(GemLotView)
		},
		{
			typeof(SummonLot),
			typeof(SummonLotView)
		},
		{
			typeof(BundleLot),
			typeof(SimpleBundleLotView)
		}
	};

	[SerializeField]
	protected List<LotView> viewPrefabs;

	[SerializeField]
	protected Transform viewContainer;

	protected DiContainer container;

	[Inject]
	public void Init(DiContainer container)
	{
		this.container = container;
	}

	public virtual LotView Create(Lot lot)
	{
		Type viewType = typeCorrespondancyDict[lot.GetType()];
		try
		{
			LotView lotView = viewPrefabs.First((LotView _view) => _view.GetType().Equals(viewType));
			LotView lotView2 = container.InstantiatePrefabForComponent<LotView>((UnityEngine.Object)lotView, viewContainer);
			lotView2.Set(lot);
			return lotView2;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Imposible to create view for LOT with ID: " + lot.ID);
		}
	}
}
