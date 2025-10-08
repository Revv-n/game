using System;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ActivityTabPimpView : MonoView
{
	[SerializeField]
	private GameObject _pimp;

	private IDisposable _disposable;

	private void OnDestroy()
	{
		_disposable?.Dispose();
	}

	public void StartTrack(MiniEventActivityTab miniEventActivityTab)
	{
		_disposable?.Dispose();
		_disposable = miniEventActivityTab.IsAnyActionAvailable.Subscribe(_pimp.SetActive);
	}
}
