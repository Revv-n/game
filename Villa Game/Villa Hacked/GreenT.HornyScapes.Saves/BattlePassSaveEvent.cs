using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
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
		IObservable<(CalendarModel, BattlePass)> observable = Observable.Where<(CalendarModel, BattlePass)>((IObservable<(CalendarModel, BattlePass)>)_battlePassProvider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Skip<bool>(Observable.SelectMany<(CalendarModel, BattlePass), bool>(observable, (Func<(CalendarModel, BattlePass), IObservable<bool>>)(((CalendarModel calendar, BattlePass battlePass) tuple) => (IObservable<bool>)tuple.battlePass.Data.StartData.PremiumPurchasedProperty)), 1), (Action<bool>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EntityStatus>(Observable.SampleFrame<EntityStatus>(Observable.SelectMany<RewardWithManyConditions, EntityStatus>(Observable.SelectMany<(CalendarModel, BattlePass), RewardWithManyConditions>(observable, (Func<(CalendarModel, BattlePass), IEnumerable<RewardWithManyConditions>>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass.AllRewardContainer.Rewards)), (Func<RewardWithManyConditions, IObservable<EntityStatus>>)((RewardWithManyConditions rewardWithManyConditions) => Observable.Skip<EntityStatus>((IObservable<EntityStatus>)rewardWithManyConditions.State, 1))), 1, (FrameCountType)0), (Action<EntityStatus>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
