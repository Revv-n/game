using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Events;

public abstract class RewardTracker : IDisposable
{
	public readonly ReactiveProperty<BaseReward> Target = new ReactiveProperty<BaseReward>();

	protected IDisposable rewardTracker;

	protected IEnumerable<BaseReward> Source { get; set; }

	public virtual void Set(IHaveTrackingRewards source)
	{
		rewardTracker?.Dispose();
		Source = source.GetRewardsToTrack();
		UpdateTargetReward(GetCurrentProgress());
		SetPreviousRewards(Target.Value);
		if (Target.Value != null)
		{
			rewardTracker = ((IObservable<BaseReward>)Target).SelectMany((Func<BaseReward, IObservable<BaseReward>>)OnRewardAchieved).Subscribe(DoOnRewardEmit);
		}
	}

	protected abstract void DoOnRewardEmit(BaseReward reward);

	protected abstract int GetCurrentProgress();

	public abstract IObservable<BaseReward> OnRewardAchieved(BaseReward reward);

	protected void UpdateTargetReward(int progress)
	{
		BaseReward baseReward = EvaluateNextReward(progress);
		if (Target.Value != baseReward)
		{
			BaseReward baseReward2 = baseReward?.PrevReward;
			while (baseReward2 != null && baseReward2 != Target.Value)
			{
				SetRewardStateOnAchieved(baseReward2);
				baseReward2 = baseReward2.PrevReward;
			}
			if (baseReward != null)
			{
				SetInProgressStateChooseNextReward(baseReward);
			}
			Target.Value = baseReward;
		}
	}

	protected abstract void SetRewardStateOnAchieved(BaseReward reward);

	protected abstract void SetInProgressStateChooseNextReward(BaseReward reward);

	protected virtual BaseReward EvaluateNextReward(int progress)
	{
		if (!Source.Any())
		{
			return null;
		}
		return Source.FirstOrDefault((BaseReward _reward) => _reward.Target > progress) ?? Source.Last();
	}

	private void SetPreviousRewards(BaseReward currentReward)
	{
		if (currentReward == null)
		{
			return;
		}
		foreach (BaseReward item in Source.TakeWhile((BaseReward _reward) => _reward != currentReward))
		{
			SetRewardStateOnAchieved(item);
		}
	}

	public virtual void Dispose()
	{
		Target?.Dispose();
		rewardTracker?.Dispose();
	}
}
