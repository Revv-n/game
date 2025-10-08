using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Tasks.UI;
using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

internal sealed class EventBattlePassNotify : BaseNotify
{
	private BattlePass _battlePass;

	private readonly Subject<bool> _onUpdate = new Subject<bool>();

	internal IObservable<bool> OnUpdate => Observable.AsObservable<bool>((IObservable<bool>)_onUpdate);

	internal void Set(BattlePass battlePass)
	{
		_battlePass = battlePass;
		ActivateNotify();
		ListenEvents();
	}

	protected override void ListenEvents()
	{
		base.ListenEvents();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<RewardWithManyConditions>(_battlePass.OnRewardUpdate, (Action<RewardWithManyConditions>)delegate
		{
			ActivateNotify();
		}), (ICollection<IDisposable>)notifyStream);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_onUpdate.Dispose();
	}

	private void ActivateNotify()
	{
		bool flag = _battlePass.HasUncollectedRewards();
		SetState(flag);
		_onUpdate.OnNext(flag);
	}
}
