using System;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public interface IConditionsLock : IDisposable
{
	void Initialization(IConditionReceivingReward conditionReceiving);

	void Reset();
}
