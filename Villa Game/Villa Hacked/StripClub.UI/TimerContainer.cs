using System;
using System.Collections.Generic;
using StripClub.Extensions;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class TimerContainer : MonoBehaviour
{
	[SerializeField]
	private TMProTimer timer;

	private TimeHelper timeHelper;

	[Inject]
	public void Init(TimeHelper timeHelper)
	{
		this.timeHelper = timeHelper;
	}

	public void SetEnableFromTimer(IEnumerable<ILocker> lockers)
	{
		TimeSpan enableFromTimeLeft = lockers.GetEnableFromTimeLeft();
		if (enableFromTimeLeft.Ticks > 0)
		{
			timer.Init(enableFromTimeLeft, timeHelper.UseCombineFormat);
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetPlaceholder(GameObject placeholder)
	{
		base.gameObject.transform.parent = placeholder.transform;
		base.gameObject.transform.position = placeholder.transform.position;
	}
}
