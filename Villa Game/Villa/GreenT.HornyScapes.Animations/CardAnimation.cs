using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Spine.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class CardAnimation : MonoBehaviour
{
	[Serializable]
	public struct Settings
	{
		public GameObject cardObject;

		public GameObject[] generalDecorationObjects;

		public GameObject[] generalDecorationBacksideObjects;

		public float waitTime;

		public Settings(GameObject cardObject, GameObject[] generalDecorationObjects, GameObject[] generalDecorationBacksideObjects, float waitTime)
		{
			this.cardObject = cardObject;
			this.generalDecorationObjects = generalDecorationObjects;
			this.generalDecorationBacksideObjects = generalDecorationBacksideObjects;
			this.waitTime = waitTime;
		}

		internal void DisplayBack(bool show)
		{
			GameObject[] array = generalDecorationBacksideObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(show);
			}
		}

		internal void DisplayFront(bool show)
		{
			GameObject[] array = generalDecorationObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(show);
			}
		}
	}

	[Header("Animation Settings")]
	[Tooltip("Каждый из этапов поворота карточки")]
	[SerializeField]
	protected float rotationDuration = 0.15f;

	[Tooltip("Время отображения карточки до того как она будет спрятана")]
	[SerializeField]
	protected float cardDisplayTime = 1f;

	[SerializeField]
	protected SkeletonAnimation skeletonAnimation;

	[SerializeField]
	protected string animationName = "animation";

	[SerializeField]
	protected RectTransform rotationContainer;

	[SerializeField]
	protected Ease ease;

	protected const float ANGLE = 90f;

	protected Quaternion initialRotation;

	protected RectTransform rectTransform;

	protected float waitTime;

	private Sequence sequence;

	private Settings settings;

	private bool isPlaying;

	public float RotationDuration => rotationDuration;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		initialRotation = rectTransform.localRotation;
	}

	internal void Init(Settings settings)
	{
		this.settings = settings;
	}

	public Sequence Play()
	{
		isPlaying = true;
		Vector3 endValue = new Vector3(0f, 90f, 0f);
		ResetAnimation();
		base.gameObject.SetActive(value: true);
		sequence = DOTween.Sequence();
		Tween t = rotationContainer.DORotate(endValue, rotationDuration).SetEase(ease);
		sequence = sequence.Append(t);
		TweenerCore<Quaternion, Quaternion, NoOptions> t2 = rotationContainer.DORotateQuaternion(initialRotation, rotationDuration).SetEase(ease);
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t3 = rotationContainer.DORotate(endValue, rotationDuration).SetEase(ease).SetDelay(waitTime);
		sequence = sequence.AppendCallback(SwitchBackToFront).Append(t2).AppendInterval(settings.waitTime)
			.Append(t3)
			.OnComplete(DeactivateAnimationObjects);
		return sequence;
		void SwitchBackToFront()
		{
			settings.DisplayBack(show: false);
			settings.DisplayFront(show: true);
			skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop: true);
		}
	}

	private void DeactivateAnimationObjects()
	{
		settings.cardObject.SetActive(value: false);
		settings.DisplayBack(show: false);
		settings.DisplayFront(show: false);
		rotationContainer.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}

	private void ResetAnimation()
	{
		settings.cardObject.SetActive(value: true);
		settings.DisplayBack(show: true);
		settings.DisplayFront(show: false);
		rotationContainer.gameObject.SetActive(value: true);
		rotationContainer.localRotation = initialRotation;
	}

	public void Stop()
	{
		if (isPlaying)
		{
			if (sequence.IsActive())
			{
				sequence.Kill();
			}
			base.gameObject.SetActive(value: false);
			DeactivateAnimationObjects();
		}
	}

	private void OnDisable()
	{
		if (isPlaying && sequence.IsActive())
		{
			sequence.Kill();
		}
	}
}
