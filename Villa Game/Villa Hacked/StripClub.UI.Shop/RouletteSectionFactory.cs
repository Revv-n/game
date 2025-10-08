using System;
using GreenT.HornyScapes.Bank.BankTabs;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class RouletteSectionFactory : MonoBehaviour, IFactory<LayoutType, RouletteLotSectionView>, IFactory
{
	[Serializable]
	public class RouletteDictionaryViewType : SerializableDictionary<LayoutType, RouletteLotSectionView>
	{
	}

	[SerializeField]
	private Transform _sectionContainer;

	[SerializeField]
	private RouletteDictionaryViewType _rouletteDictionaryViewType;

	private DiContainer _container;

	[Inject]
	public void Init(DiContainer container)
	{
		_container = container;
	}

	public RouletteLotSectionView Create(LayoutType param)
	{
		if (!_rouletteDictionaryViewType.TryGetValue(param, out var value))
		{
			throw new NotImplementedException("There is no prefab for this type of Layout: " + param);
		}
		return _container.InstantiatePrefabForComponent<RouletteLotSectionView>((UnityEngine.Object)value, _sectionContainer);
	}
}
