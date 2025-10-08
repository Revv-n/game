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
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		return type switch
		{
			CardViewType.Free => _container.InstantiatePrefabForComponent<BattlePassRewardCard>((Object)_freeCard), 
			CardViewType.Premium => _container.InstantiatePrefabForComponent<BattlePassRewardCard>((Object)_premiumCard), 
			_ => throw new SwitchExpressionException((object)type), 
		};
	}
}
