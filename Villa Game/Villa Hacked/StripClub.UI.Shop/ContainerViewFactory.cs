using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class ContainerViewFactory : MonoBehaviour, IFactory<LotContainer, ContainerView>, IFactory
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

	public virtual ContainerView Create(LotContainer lotLotContainer)
	{
		Type viewType = typeCorrespondancyDict[lotLotContainer.Lots.ToList()[0].GetType()];
		try
		{
			LotView lotView = viewPrefabs.First((LotView _view) => _view.GetType().Equals(viewType));
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
