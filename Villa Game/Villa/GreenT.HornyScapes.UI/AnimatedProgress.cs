using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class AnimatedProgress : MonoBehaviour, IInitializable, ISerializationCallbackReceiver
{
	[SerializeField]
	private FloatClampedProgress progress;

	[SerializeField]
	private float startDelay;

	[SerializeField]
	private float fillDuration = 1f;

	[SerializeField]
	private EasingFunction.Ease easingFunction = EasingFunction.Ease.EaseInOutQuad;

	private Subject<float> progresSubject = new Subject<float>();

	private EasingFunction.Function function;

	private CompositeDisposable disposables = new CompositeDisposable();

	public IObservable<float> OnProgressComplete => progresSubject.AsObservable();

	public double Progress => progress.Progress;

	public bool IsComplete()
	{
		return progress.IsComplete();
	}

	private void OnValidate()
	{
		if (!progress)
		{
			progress = GetComponent<FloatClampedProgress>();
		}
		Initialize();
	}

	protected virtual void Awake()
	{
		Initialize();
	}

	public void AnimateFromCurrent(int newValue, int maxValue, int minValue = 0)
	{
		Set(newValue, maxValue, minValue, fromCurrentProgress: true);
	}

	public void AnimateFromZero(int newValue, int maxValue, int minValue = 0)
	{
		Set(newValue, maxValue, minValue);
	}

	public void Set(int newValue, int maxValue, int minValue, bool fromCurrentProgress = false)
	{
		float currentAnimationTime = 0f;
		float startPoint = ((fromCurrentProgress && minValue < maxValue) ? Mathf.Lerp(minValue, maxValue, progress.Progress) : ((float)minValue));
		disposables?.Clear();
		Observable.EveryUpdate().Skip(TimeSpan.FromSeconds(startDelay)).TakeWhile((long _) => currentAnimationTime < fillDuration)
			.Subscribe(delegate
			{
				currentAnimationTime += Time.deltaTime;
				float value = function(startPoint, newValue, currentAnimationTime / fillDuration);
				progress.Init(value, maxValue, minValue);
			}, delegate
			{
				UpdateProgress(newValue, maxValue, minValue);
			}, delegate
			{
				UpdateProgress(newValue, maxValue, minValue);
			})
			.AddTo(disposables);
	}

	private void UpdateProgress(float current, int max, int min)
	{
		progress.Init(current, max, min);
		progresSubject.OnNext(current);
	}

	private void OnDestroy()
	{
		disposables.Dispose();
	}

	public void Initialize()
	{
		function = EasingFunction.GetEasingFunction(easingFunction);
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Initialize();
	}

	private void OnDisable()
	{
	}
}
