using System;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views.Aminations;

public sealed class LovePointsClip : Clip
{
	[SerializeField]
	private LovePointsAnimateCollect _animateCollect;

	[SerializeField]
	private ImagePool _pool;

	[SerializeField]
	private Sprite _sprite;

	[Header("Animation Settings")]
	[SerializeField]
	private float _waitOnTheEnd = 0.3f;

	[SerializeField]
	private float _flyResourcesDelay = 1.8f;

	public void Init(Transform startPoint, Transform endPoint)
	{
		_animateCollect.Init(_pool, startPoint, endPoint, _sprite);
	}

	public override void Play()
	{
		base.gameObject.SetActive(value: true);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Take<long>(Observable.Timer(TimeSpan.FromSeconds(_flyResourcesDelay)), 1), (Component)this), (Action<long>)delegate
		{
			LaunchCollectAnimation();
		}), (Component)this);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}

	private void LaunchCollectAnimation()
	{
		_animateCollect.Launch();
	}
}
