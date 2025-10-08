using DG.Tweening;
using GreenT.HornyScapes.Animations;
using StripClub.Model;
using TMPro;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class AddBattlePassLevelClip : Clip
{
	[SerializeField]
	private CardBattlePassLevelView cardView;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation cardAnimation;

	[SerializeField]
	private TMP_Text quantity;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	[Header("Animation Settings")]
	[SerializeField]
	private float waitOnTheEnd = 0.3f;

	public void Init(BattlePassLevelLinkedContent content)
	{
		cardView.Set(content);
		quantity.gameObject.SetActive(value: false);
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(cardView.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitOnTheEnd);
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
