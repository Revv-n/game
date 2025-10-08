using System;
using GreenT.HornyScapes.Constants;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipOpenerGirlClick : GirlToolTipOpener, IPointerDownHandler, IEventSystemHandler
{
	private float dropChance;

	[Inject]
	private void Init(IConstants<float> floatConstants, IConstants<int> intConstants)
	{
		dropChance = floatConstants["phrase_tap_chance"];
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		base.ReadyToActivate.Value = false;
		if (!(UnityEngine.Random.Range(0f, 1f) <= dropChance) && !base.IsPlaying.Value)
		{
			base.ReadyToActivate.Value = true;
		}
	}
}
