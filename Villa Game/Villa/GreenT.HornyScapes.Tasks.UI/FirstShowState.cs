using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class FirstShowState : TaskViewState
{
	public Animation childFollowAnimController;

	public RectTransformAnimation showAnimation;

	public LayoutElement LayoutElement;

	public ButtonStrategy ButtonStrategy;

	protected void Awake()
	{
		showAnimation.Init();
	}

	public override void Enter()
	{
		base.Enter();
		LayoutElement.ignoreLayout = false;
		ButtonStrategy.SetInteractable(isOn: false);
		Observable.TimerFrame(1).Subscribe(delegate
		{
			childFollowAnimController.Stop();
			showAnimation.ResetToAnimStart();
			showAnimation.Play();
			ButtonStrategy.CheckInteractable();
		}).AddTo(this);
	}
}
