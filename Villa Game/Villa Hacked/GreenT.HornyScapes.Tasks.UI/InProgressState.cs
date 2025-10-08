namespace GreenT.HornyScapes.Tasks.UI;

public class InProgressState : TaskViewState
{
	public CompletableView CompletableView;

	public ButtonStrategy ButtonStrategy;

	public override void Enter()
	{
		base.Enter();
		CompletableView.SetComplete(source.ReadyToComplete);
		ButtonStrategy.SetState(source.ReadyToComplete);
	}
}
