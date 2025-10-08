using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Collections.Promote.UI.Animation;

public class AnimatedPromoteCardView : PromoteCardView
{
	[SerializeField]
	private List<AnimationDependedStarter> animatinStartes = new List<AnimationDependedStarter>();

	[SerializeField]
	private List<AnimationStartersBind> bindStarters = new List<AnimationStartersBind>();

	private CompositeDisposable onLevelUpStream = new CompositeDisposable();

	protected void OnValidate()
	{
		if (animatinStartes == null || animatinStartes.Count == 0)
		{
			Debug.LogError("Starters list is empty", this);
		}
	}

	public override void Set(ICard card)
	{
		base.Set(card);
		SetAnimationSettings(card);
	}

	private void SetAnimationSettings(ICard card)
	{
		foreach (AnimationDependedStarter animatinStarte in animatinStartes)
		{
			animatinStarte.Init();
		}
	}

	public void AnimateCard()
	{
		onLevelUpStream?.Clear();
		foreach (AnimationStartersBind bindStarter in bindStarters)
		{
			bindStarter.Bind();
		}
		animatinStartes[0].LaunchStarter();
	}
}
