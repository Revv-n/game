using System;
using StripClub.UI;
using UniRx;
using UnityEngine;

public class AfterShowNewGirlClip : Clip
{
	[SerializeField]
	private float duration;

	public override void Play()
	{
		base.gameObject.SetActive(value: true);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Timer(TimeSpan.FromSeconds(duration)), (Component)this), (Action<long>)delegate
		{
			Hide();
		}), (Component)this);
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
		Stop();
	}
}
