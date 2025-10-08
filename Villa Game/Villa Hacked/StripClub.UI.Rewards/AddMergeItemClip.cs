using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Content;
using TMPro;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class AddMergeItemClip : Clip
{
	[SerializeField]
	private RewardMergeItemCardView mergeItemCard;

	[SerializeField]
	private float waitTime = 1f;

	[SerializeField]
	private TMP_Text quantity;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	private MergeItemLinkedContent item;

	public void Init(MergeItemLinkedContent mergeItemLinkedContent)
	{
		item = mergeItemLinkedContent;
		quantity.text = "+" + mergeItemLinkedContent.Quantity;
		mergeItemCard.Set(item);
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(mergeItemCard.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitTime);
		cardAnimation.Init(settings);
	}

	public override void Play()
	{
		cardAnimation.Play().OnComplete(Stop);
	}

	public override void Stop()
	{
		mergeItemCard.Hide();
		cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}
}
