namespace GreenT.HornyScapes.MiniEvents;

public sealed class RewardTaskState : MiniEventBaseTaskStateView
{
	public override void Enter()
	{
		_buttonSpriteStates.Set(_buttonState);
		_textMeshProValueStates.Set(_buttonTextState);
		_buttonStrategy.SetInteractable(source.ReadyToComplete);
	}
}
