using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Collections.Promote.UI.Animation;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Collections.Promote.UI;

public class AnimatedPromoteButton : PromoteButton
{
	[SerializeField]
	private AnimationController promoteOnEndController;

	[SerializeField]
	private AnimationStarter starter;

	[SerializeField]
	private AnimatedPromoteCardView cardClap;

	private CompositeDisposable animationStream = new CompositeDisposable();

	protected void Start()
	{
		starter.Init();
	}

	protected override void Promote()
	{
		promoteButton.enabled = false;
		starter.PlayAnimation();
		cardClap.AnimateCard();
		starter.OnEnd.First().DoOnCancel(starter.ResetAnimation).DoOnCompleted(starter.ResetAnimation)
			.Subscribe()
			.AddTo(animationStream);
		promoteOnEndController.OnPlayEnd.First().DoOnCancel(delegate
		{
			base.Promote();
			promoteButton.enabled = true;
		}).Subscribe(delegate
		{
			base.Promote();
			promoteButton.enabled = true;
		})
			.AddTo(animationStream);
	}
}
