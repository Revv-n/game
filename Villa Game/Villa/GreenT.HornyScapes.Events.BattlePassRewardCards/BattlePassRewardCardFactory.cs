using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassRewardCardFactory : MonoBehaviour, IFactory<BattlePassRewardCardFactory.CardViewType, BattlePassRewardCard>, IFactory
{
	public enum CardViewType
	{
		Free,
		Premium
	}

	[SerializeField]
	private BattlePassRewardCard _freeCard;

	[SerializeField]
	private BattlePassRewardCard _premiumCard;

	private DiContainer _container;

	[Inject]
	private void Construct(DiContainer container)
	{
		_container = container;
	}

	public BattlePassRewardCard Create(CardViewType type)
	{
		return type switch
		{
			CardViewType.Free => _container.InstantiatePrefabForComponent<BattlePassRewardCard>(_freeCard), 
			CardViewType.Premium => _container.InstantiatePrefabForComponent<BattlePassRewardCard>(_premiumCard), 
			_ => throw new SwitchExpressionException(type), 
		};
	}
}
