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
		Observable.Timer(TimeSpan.FromSeconds(duration)).TakeUntilDisable(this).Subscribe(delegate
		{
			Stop();
		})
			.AddTo(this);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		base.Stop();
	}
}
