using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class DuplicateCardAnimation : Animation
{
	public struct CardSettings
	{
		public GameObject[] DuplicateFrontObjects;

		public GameObject[] DuplicateBacksideObjects;

		public GameObject[] RewardFrontObjects;

		public GameObject[] RewardBacksideObjects;

		public GameObject Reward;

		public GameObject AlternativeReward;

		public float WaitTime;

		public CardSettings(GameObject reward, GameObject alternativeReward, float waitTime, GameObject[] duplicateFrontObjects, GameObject[] duplicateBacksideObjects, GameObject[] rewardFrontObjects, GameObject[] rewardBacksideObjects)
		{
			Reward = reward;
			AlternativeReward = alternativeReward;
			DuplicateFrontObjects = duplicateFrontObjects;
			DuplicateBacksideObjects = duplicateBacksideObjects;
			RewardFrontObjects = rewardFrontObjects;
			RewardBacksideObjects = rewardBacksideObjects;
			WaitTime = waitTime;
		}

		internal void DisplayDuplicateBack(bool state)
		{
			GameObject[] duplicateBacksideObjects = DuplicateBacksideObjects;
			for (int i = 0; i < duplicateBacksideObjects.Length; i++)
			{
				duplicateBacksideObjects[i].SetActive(state);
			}
		}

		internal void DisplayDuplicateFront(bool state)
		{
			GameObject[] duplicateFrontObjects = DuplicateFrontObjects;
			for (int i = 0; i < duplicateFrontObjects.Length; i++)
			{
				duplicateFrontObjects[i].SetActive(state);
			}
		}

		internal void DisplayRewardBack(bool state)
		{
			GameObject[] rewardBacksideObjects = RewardBacksideObjects;
			for (int i = 0; i < rewardBacksideObjects.Length; i++)
			{
				rewardBacksideObjects[i].SetActive(state);
			}
		}

		internal void DisplayRewardFront(bool state)
		{
			GameObject[] rewardFrontObjects = RewardFrontObjects;
			for (int i = 0; i < rewardFrontObjects.Length; i++)
			{
				rewardFrontObjects[i].SetActive(state);
			}
		}
	}

	[SerializeField]
	private float _rotationDuration = 0.15f;

	[SerializeField]
	private float _cardDisplayTime = 1f;

	[SerializeField]
	private string _animationName = "animation";

	[SerializeField]
	private RectTransform _rotationContainer;

	[SerializeField]
	private Ease _ease;

	[SerializeField]
	private float _angle = 90f;

	[SerializeField]
	private GameObject _duplicateView;

	[SerializeField]
	private GameObject _exchangeView;

	private Vector3 _initialRotation;

	private RectTransform _rectTransform;

	private Sequence _sequence;

	private CardSettings _settings;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_initialRotation = _rectTransform.localRotation.eulerAngles;
	}

	public void Init(CardSettings settings)
	{
		base.Init();
		_settings = settings;
	}

	public override Sequence Play()
	{
		ResetToAnimStart();
		_sequence = DOTween.Sequence();
		_sequence.Append(PlayDuplicateSequence());
		_sequence.Append(PlayRewardSequence());
		_sequence.AppendCallback(Stop);
		return _sequence;
	}

	private Sequence PlayDuplicateSequence()
	{
		Vector3 endValue = new Vector3(0f, _angle, 0f);
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t = _rotationContainer.DORotate(endValue, _rotationDuration).SetEase(_ease);
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t2 = _rotationContainer.DORotate(_initialRotation, _rotationDuration).SetEase(_ease);
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t3 = _rotationContainer.DORotate(endValue, _rotationDuration).SetEase(_ease).SetDelay(_settings.WaitTime);
		return DOTween.Sequence().Append(t).AppendCallback(ShowFront)
			.Append(t2)
			.AppendInterval(_settings.WaitTime)
			.Append(t3);
		void ShowFront()
		{
			_duplicateView.SetActive(value: true);
			_settings.DisplayDuplicateBack(state: false);
			_settings.DisplayDuplicateFront(state: true);
		}
	}

	private Sequence PlayRewardSequence()
	{
		TweenerCore<Quaternion, Vector3, QuaternionOptions> t = _rotationContainer.DORotate(_initialRotation, _rotationDuration).SetEase(_ease);
		return DOTween.Sequence().AppendCallback(DisableDuplicateCard).AppendCallback(EnableRewardCard)
			.AppendCallback(ShowFront)
			.Append(t)
			.AppendInterval(_settings.WaitTime);
		void ShowFront()
		{
			_exchangeView.SetActive(value: true);
			_settings.DisplayRewardBack(state: false);
			_settings.DisplayRewardFront(state: true);
		}
	}

	public override void ResetToAnimStart()
	{
		_settings.Reward.SetActive(value: true);
		_settings.DisplayDuplicateBack(state: true);
		_settings.DisplayDuplicateFront(state: false);
		DisableRewardCard();
		base.gameObject.SetActive(value: true);
		_rotationContainer.gameObject.SetActive(value: true);
		_rotationContainer.eulerAngles = _initialRotation;
	}

	public override void Stop()
	{
		base.Stop();
		DisableAnimationObjects();
	}

	private void DisableAnimationObjects()
	{
		DisableDuplicateCard();
		DisableRewardCard();
		base.gameObject.SetActive(value: false);
		_rotationContainer.gameObject.SetActive(value: false);
	}

	private void DisableDuplicateCard()
	{
		if (!(_settings.Reward == null))
		{
			_settings.Reward.SetActive(value: false);
			_settings.DisplayDuplicateBack(state: false);
			_settings.DisplayDuplicateFront(state: false);
			_duplicateView.SetActive(value: false);
		}
	}

	private void DisableRewardCard()
	{
		if (!(_settings.AlternativeReward == null))
		{
			_settings.AlternativeReward.SetActive(value: false);
			_settings.DisplayRewardBack(state: false);
			_settings.DisplayRewardFront(state: false);
			_exchangeView.SetActive(value: false);
		}
	}

	private void EnableRewardCard()
	{
		_settings.AlternativeReward.SetActive(value: true);
		_settings.DisplayRewardBack(state: true);
		_settings.DisplayRewardFront(state: false);
	}
}
