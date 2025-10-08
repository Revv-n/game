using System;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class NotifyReward : BaseReward
{
	private Subject<NotifyReward> onUpdate = new Subject<NotifyReward>();

	public IObservable<NotifyReward> OnUpdate => onUpdate.AsObservable();

	public NotifyReward(LinkedContent content, int target, NotifyReward prevReward)
		: base(content, target, prevReward)
	{
	}

	public override void SetRewarded()
	{
		base.SetRewarded();
		onUpdate.OnNext(this);
	}

	public override void SetInProgress()
	{
		base.SetInProgress();
		onUpdate.OnNext(this);
	}

	public override void Dispose()
	{
		base.Dispose();
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}
}
