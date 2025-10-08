namespace GreenT.HornyScapes.Tasks.UI;

public class CompleteState : TaskViewState
{
	public ButtonStrategy ButtonStrategy;

	public CompletableView CompletableView;

	public override void Enter()
	{
		base.Enter();
		CompletableView.SetComplete(source.ReadyToComplete);
		ButtonStrategy.SetState(source.ReadyToComplete);
		ButtonStrategy.SetInteractable(isOn: true);
	}

	public override void Exit()
	{
		base.Exit();
		CompletableView.SetComplete(source.ReadyToComplete);
		ButtonStrategy.SetState(source.ReadyToComplete);
		ButtonStrategy.SetInteractable(isOn: false);
	}
}
