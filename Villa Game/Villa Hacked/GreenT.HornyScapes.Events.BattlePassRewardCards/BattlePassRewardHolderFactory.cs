using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassRewardHolderFactory : MonoBehaviour, IFactory<BattlePassRewardPairData, BattlePass, BaseBattlePassRewardHolder>, IFactory
{
	[Serializable]
	public class HolderViewType : SerializableDictionary<BattlePassRewardViewType, BaseBattlePassRewardHolder>
	{
	}

	[Serializable]
	public class CardFactoryType : SerializableDictionary<BattlePassRewardViewType, BattlePassRewardCardFactory>
	{
	}

	[SerializeField]
	private CardFactoryType _cardViewMap;

	[SerializeField]
	private HolderViewType _holderViewMap;

	private BattlePassProvider _battlePassProvider;

	private DiContainer _container;

	[Inject]
	public void Construct(DiContainer container)
	{
		_container = container;
	}

	public BaseBattlePassRewardHolder Create(BattlePassRewardPairData param, BattlePass battlePass)
	{
		BattlePassBundleData bundle = battlePass.Bundle;
		if (bundle == null || !_holderViewMap.TryGetValue(param.GetViewType(), out var value) || !_cardViewMap.TryGetValue(param.GetViewType(), out var value2))
		{
			throw new NotImplementedException($"There is no prefab for this type of Layout: '{param}'");
		}
		BaseBattlePassRewardHolder baseBattlePassRewardHolder = _container.InstantiatePrefabForComponent<BaseBattlePassRewardHolder>((UnityEngine.Object)value, (IEnumerable<object>)new object[1] { value2 });
		baseBattlePassRewardHolder.SetBundle(bundle);
		baseBattlePassRewardHolder.Set(param);
		return baseBattlePassRewardHolder;
	}
}
