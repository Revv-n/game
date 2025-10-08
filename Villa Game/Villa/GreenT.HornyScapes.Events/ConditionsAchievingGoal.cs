using UniRx;

namespace GreenT.HornyScapes.Events;

public class ConditionsAchievingGoal : BaseConditionReceivingReward<ConditionsAchievingGoal>
{
	private readonly IReadOnlyReactiveProperty<bool> _goalStatus;

	private bool IsGoalAchieved => _goalStatus.Value;

	public override string ConditionText { get; }

	public ConditionsAchievingGoal(IReadOnlyReactiveProperty<bool> goalStatus, string conditionText)
	{
		_goalStatus = goalStatus;
		ConditionText = conditionText;
	}

	public override bool Validate()
	{
		return OnComplete();
	}

	protected override bool CheckIfCompleted()
	{
		if (!IsGoalAchieved)
		{
			return false;
		}
		SetCompleted();
		return true;
	}

	protected override void Subscribe()
	{
		SubscribeDispose = _goalStatus.Where((bool p) => p).Subscribe(delegate(bool p)
		{
			OnComplete(p);
		});
	}

	private bool OnComplete()
	{
		return OnComplete(_goalStatus.Value);
	}

	private bool OnComplete(bool value)
	{
		if (IsDisabled())
		{
			return false;
		}
		if (IsCompleted())
		{
			return true;
		}
		if (value && !IsCompleted())
		{
			SetCompleted();
		}
		return value;
	}
}
