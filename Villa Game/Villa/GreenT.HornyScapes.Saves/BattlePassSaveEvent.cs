using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class BattlePassSaveEvent : SaveEvent
{
	private BattlePassProvider _battlePassProvider;

	[Inject]
	private void Init(BattlePassProvider battlePassProvider)
	{
		_battlePassProvider = battlePassProvider;
	}

	public override void Track()
	{
		IObservable<(CalendarModel calendar, BattlePass battlePass)> source = _battlePassProvider.CalendarChangeProperty.Where(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null);
		source.SelectMany(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass.Data.StartData.PremiumPurchasedProperty).Skip(1).Subscribe(delegate
		{
			Save();
		})
			.AddTo(saveStreams);
		source.SelectMany(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass.AllRewardContainer.Rewards).SelectMany((RewardWithManyConditions rewardWithManyConditions) => rewardWithManyConditions.State.Skip(1)).SampleFrame(1)
			.Subscribe(delegate
			{
				Save();
			})
			.AddTo(saveStreams);
	}
}
