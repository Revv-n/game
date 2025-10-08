using System;
using System.Collections.Generic;
using UnityEngine;

namespace StripClub.UI;

public class TMProTimerWithObjects : TMProTimer
{
	[SerializeField]
	private List<GameObject> objects = new List<GameObject>();

	public override void Init(TimeSpan timeLeft, Func<TimeSpan, string> timerFormat)
	{
		ActivateTimerObject(timeLeft.Ticks > 0);
		base.Init(timeLeft, timerFormat);
	}

	protected override void OnTimerSwitchState(bool isActive)
	{
		base.OnTimerSwitchState(isActive);
		ActivateTimerObject(isActive);
	}

	private void ActivateTimerObject(bool active)
	{
		foreach (GameObject @object in objects)
		{
			@object.SetActive(active);
		}
	}
}
