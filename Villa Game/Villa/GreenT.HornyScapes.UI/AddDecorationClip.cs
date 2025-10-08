using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class AddDecorationClip : Clip
{
	[SerializeField]
	private RewardDecorationCardView decorationCard;

	[SerializeField]
	private float waitTime = 1f;

	[SerializeField]
	private CardAnimation cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	public void Init(DecorationLinkedContent decorationLinkedContent)
	{
		decorationCard.Set(decorationLinkedContent);
		CardAnimation.Settings settings = new CardAnimation.Settings(decorationCard.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitTime);
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
