using System;
using GreenT.HornyScapes.Bank.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferSectionFactory : MonoBehaviour, IFactory<LayoutType, OfferSectionView>, IFactory
{
	[Serializable]
	public class DictionaryViewType : SerializableDictionary<LayoutType, OfferSectionView>
	{
	}

	[SerializeField]
	private Transform sectionContainer;

	[SerializeField]
	private DictionaryViewType viewByTypeDictionary;

	private DiContainer container;

	[Inject]
	public void Init(DiContainer container)
	{
		this.container = container;
	}

	public OfferSectionView Create(LayoutType param)
	{
		if (!viewByTypeDictionary.TryGetValue(param, out var value))
		{
			throw new NotImplementedException("There is no prefab for this type of Layout: '" + param.ToString() + "'");
		}
		return container.InstantiatePrefabForComponent<OfferSectionView>((UnityEngine.Object)value, sectionContainer);
	}
}
