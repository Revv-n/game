using System;
using GreenT.HornyScapes.ToolTips;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

public class ToolTipUISound : ToolTipSound
{
	[SerializeField]
	protected ToolTipUIOpener _source;

	protected override void ValidateToolTip()
	{
		GetComponent<ToolTipUIOpener>(ref _source);
		void GetComponent<T>(ref T component) where T : MonoBehaviour
		{
			if (!(component != null) && !TryGetComponent<T>(out component))
			{
				Debug.LogError("Empty component: " + component.GetType(), this);
			}
		}
	}

	protected override IObservable<Unit> OnOpenTrigger()
	{
		return _source.OnOpen.AsUnitObservable();
	}

	protected override IObservable<Unit> OnCloseTrigger()
	{
		return _source.OnClose.AsUnitObservable();
	}
}
