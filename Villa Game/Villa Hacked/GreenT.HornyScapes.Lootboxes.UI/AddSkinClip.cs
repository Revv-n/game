using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Sounds;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Lootboxes.UI;

public class AddSkinClip : Clip
{
	[SerializeField]
	private SkinView skinView;

	[SerializeField]
	private CardAnimation cardAnimation;

	[SerializeField]
	private TMP_Text quantity;

	[SerializeField]
	private float waitOnTheEnd = 1.8f;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[SerializeField]
	private StatableComponent frontRarityStatable;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	[SerializeField]
	private CollectionSoundSO audioClips;

	private void OnValidate()
	{
		if (audioClips == null)
		{
			Debug.LogError("Lootboxes: AddSoulsClips has empty audio");
		}
	}

	public override void Play()
	{
		cardAnimation.Play().OnComplete(Stop);
	}

	public void Init(SkinLinkedContent skinContent)
	{
		Rarity rarity = skinContent.Skin.Rarity;
		skinView.Set(skinContent.Skin);
		skinView.gameObject.SetActive(value: true);
		Random.Range(0, audioClips.Sounds.Count);
		quantity.text = "+1";
		frontRarityStatable.Set((int)rarity);
		CardAnimation.Settings settings = new CardAnimation.Settings(skinView.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitOnTheEnd);
		cardAnimation.Init(settings);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		skinView.gameObject?.SetActive(value: false);
	}
}
