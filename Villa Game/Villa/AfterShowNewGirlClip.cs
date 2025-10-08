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
		Observable.Timer(TimeSpan.FromSeconds(duration)).TakeUntilDisable(this).Subscribe(delegate
		{
			Hide();
		})
			.AddTo(this);
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
		Stop();
	}
}
