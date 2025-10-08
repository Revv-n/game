using DG.Tweening;
using GreenT.Bonus;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Booster;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class AddBoosterClip : Clip
{
	[SerializeField]
	private CardBoosterView boosterView;

	[Header("Animation Settings")]
	[SerializeField]
	private float waitOnTheEnd = 0.3f;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	private BonusType bonus;

	public void Init(BoosterLinkedContent boosterContent)
	{
		bonus = boosterContent.BonusType;
		boosterView.Set(bonus);
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(boosterView.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitOnTheEnd);
		cardAnimation.Init(settings);
	}

	public override void Play()
	{
		cardAnimation.Play().OnComplete(Stop);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}
}
