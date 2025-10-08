using System;
using UniRx;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class ActivateObjectOnTimeClip : Clip
{
	[SerializeField]
	private float duration = 1f;

	public override void Play()
	{
		base.gameObject.SetActive(value: true);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Timer(TimeSpan.FromSeconds(duration)), (Component)this), (Action<long>)delegate
		{
			Stop();
		}), (Component)this);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		base.Stop();
	}
}
