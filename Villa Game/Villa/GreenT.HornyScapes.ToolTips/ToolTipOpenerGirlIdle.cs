using System;
using GreenT.HornyScapes.Constants;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipOpenerGirlIdle : GirlToolTipOpener
{
	private IDisposable idleDisposable;

	private IConstants<int> intConstants;

	[Inject]
	private void Init(IConstants<int> intConstants)
	{
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
		this.intConstants = intConstants;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		RestartTimer();
	}

	private void RestartTimer()
	{
		int minInclusive = intConstants["phrase_delaymin"];
		int maxExclusive = intConstants["phrase_delaymax"];
		int num = UnityEngine.Random.Range(minInclusive, maxExclusive);
		idleDisposable?.Dispose();
		idleDisposable = Observable.Timer(TimeSpan.FromSeconds(num)).Subscribe(delegate
		{
			base.ReadyToActivate.Value = true;
		}).AddTo(this);
	}
}
