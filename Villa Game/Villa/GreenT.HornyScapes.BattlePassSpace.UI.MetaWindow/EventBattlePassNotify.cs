using System;
using GreenT.HornyScapes.Tasks.UI;
using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

internal sealed class EventBattlePassNotify : BaseNotify
{
	private BattlePass _battlePass;

	private readonly Subject<bool> _onUpdate = new Subject<bool>();

	internal IObservable<bool> OnUpdate => _onUpdate.AsObservable();

	internal void Set(BattlePass battlePass)
	{
		_battlePass = battlePass;
		ActivateNotify();
		ListenEvents();
	}

	protected override void ListenEvents()
	{
		base.ListenEvents();
		_battlePass.OnRewardUpdate.Subscribe(delegate
		{
			ActivateNotify();
		}).AddTo(notifyStream);
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
