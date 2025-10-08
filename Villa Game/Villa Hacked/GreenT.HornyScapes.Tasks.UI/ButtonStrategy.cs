namespace GreenT.HornyScapes.Tasks.UI;

public class ButtonStrategy : BaseButtonStrategy
{
	public TaskView TaskView;

	protected override void SetRewardState()
	{
		TaskView.TaskViewStateMachine.ForceSetRewardState();
	}
}
