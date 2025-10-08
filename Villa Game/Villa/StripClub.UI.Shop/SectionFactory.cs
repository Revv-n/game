using System;
using GreenT.HornyScapes.Bank.BankTabs;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SectionFactory : MonoBehaviour, IFactory<LayoutType, BankSectionView>, IFactory
{
	[Serializable]
	public class DictionaryViewType : SerializableDictionary<LayoutType, BankSectionView>
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

	public BankSectionView Create(LayoutType param)
	{
		if (!viewByTypeDictionary.TryGetValue(param, out var value))
		{
			throw new NotImplementedException("There is no prefab for this type of Layout: " + param);
		}
		return container.InstantiatePrefabForComponent<BankSectionView>(value, sectionContainer);
	}
}
