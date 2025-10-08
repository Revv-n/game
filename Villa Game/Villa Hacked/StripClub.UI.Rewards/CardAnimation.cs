using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Spine.Unity;
using StripClub.Model.Cards;
using StripClub.UI.Rewards.Animation.Settings;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class CardAnimation : MonoBehaviour
{
	[Serializable]
	public struct AnimEffect
	{
		public GameObject[] Effects;
	}

	[Serializable]
	public class RarityRewardDictionary : SerializableDictionary<Rarity, AnimEffect>
	{
	}

	[Header("Animation Settings")]
	[SerializeField]
	protected float rotationDuration = 0.15f;

	[SerializeField]
	protected float effectOnDelay = 0.07f;

	[SerializeField]
	protected GameObject skeletonAnimationContainer;

	[SerializeField]
	protected SkeletonAnimation skeletonAnimation;

	[SerializeField]
	protected string animationName = "animation";

	[SerializeField]
	protected RectTransform rotationContainer;

	[SerializeField]
	protected GameObject quantityText;

	[SerializeField]
	protected Ease ease;

	protected const float ANGLE = 90f;

	protected Quaternion initialRotation;

	protected RectTransform rectTransform;

	[SerializeField]
	private RarityRewardDictionary rarityEffectsSettings;

	private CardAnimSettings objectsSettings;

	private bool isFastRotate;

	protected float waitTime;

	private Sequence sequence;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		initialRotation = rectTransform.localRotation;
	}

	[Obsolete]
	public void Init(CardAnimSettings settings, float waitTime)
	{
		objectsSettings = settings;
		this.waitTime = waitTime;
	}

	[Obsolete]
	public void Init(CardAnimSettings settings, float waitTime, Rarity rarity)
	{
		Init(settings, waitTime);
		settings.Effects = rarityEffectsSettings[rarity].Effects;
		isFastRotate = rarity == Rarity.Rare;
	}

	public void Play(Action onComplete = null)
	{
		Vector3 endValue = new Vector3(0f, 90f, 0f);
		ResetCard();
		base.gameObject.SetActive(value: true);
		sequence = DOTween.Sequence();
		Tween t = rotationContainer.DORotate(endValue, rotationDuration).SetEase(ease);
		sequence = sequence.Append(t);
		TweenerCore<Quaternion, Quaternion, NoOptions> t2 = rotationContainer.DORotateQuaternion(initialRotation, rotationDuration).SetEase(ease);
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t3 = rotationContainer.DORotate(endValue, rotationDuration).SetEase(ease).SetDelay(waitTime);
		sequence = sequence.AppendCallback(DeactivateBackplate).Append(t2).AppendInterval(effectOnDelay)
			.Append(t3)
			.OnComplete(DeactivateAnimationObjects)
			.OnComplete(delegate
			{
				onComplete?.Invoke();
			});
		void DeactivateAnimationObjects()
		{
			GameObject[] effects = objectsSettings.Effects;
			for (int i = 0; i < effects.Length; i++)
			{
				effects[i].SetActive(value: false);
			}
			objectsSettings.Back.SetActive(value: false);
			objectsSettings.Flare.SetActive(value: false);
			skeletonAnimationContainer.SetActive(value: true);
			objectsSettings.Front.SetActive(value: false);
			rotationContainer.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
		}
		void DeactivateBackplate()
		{
			objectsSettings.Back.SetActive(value: false);
			objectsSettings.Front.SetActive(value: true);
			objectsSettings.Flare.SetActive(value: true);
			GameObject[] effects2 = objectsSettings.Effects;
			for (int j = 0; j < effects2.Length; j++)
			{
				effects2[j].SetActive(value: true);
			}
			skeletonAnimationContainer.SetActive(value: true);
			skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop: true);
		}
	}

	private void ResetCard()
	{
		if (base.gameObject.activeSelf)
		{
			objectsSettings.Back.SetActive(value: true);
			GameObject[] effects = objectsSettings.Effects;
			for (int i = 0; i < effects.Length; i++)
			{
				effects[i].SetActive(value: false);
			}
			quantityText.SetActive(value: false);
			objectsSettings.Flare.SetActive(value: false);
			objectsSettings.Front.SetActive(value: true);
			skeletonAnimationContainer.SetActive(value: false);
			rotationContainer.gameObject.SetActive(value: true);
			rotationContainer.localRotation = initialRotation;
		}
	}

	public void Stop()
	{
		base.gameObject.SetActive(value: false);
		ResetCard();
	}
}
