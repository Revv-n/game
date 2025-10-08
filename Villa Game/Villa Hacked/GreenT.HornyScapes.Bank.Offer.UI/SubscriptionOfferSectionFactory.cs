using System;
using GreenT.HornyScapes.Bank.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SubscriptionOfferSectionFactory : MonoBehaviour, IFactory<LayoutType, SubscriptionOfferSectionView>, IFactory
{
	[Serializable]
	public class DictionaryViewType : SerializableDictionary<LayoutType, SubscriptionOfferSectionView>
	{
	}

	[SerializeField]
	private Transform sectionContainer;

	[SerializeField]
	private DictionaryViewType viewByTypeDictionary;

	private DiContainer _container;

	[Inject]
	public void Init(DiContainer container)
	{
		_container = container;
	}

	public SubscriptionOfferSectionView Create(LayoutType param)
	{
		if (!viewByTypeDictionary.TryGetValue(param, out var value))
		{
			throw new NotImplementedException("There is no prefab for this type of Layout: '" + param.ToString() + "'");
		}
		return _container.InstantiatePrefabForComponent<SubscriptionOfferSectionView>((UnityEngine.Object)value, sectionContainer);
	}
}
