using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Meta.RoomObjects;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class GirlToolTipController : MonoBehaviour
{
	[Serializable]
	public class TooltipDictionary : SerializableDictionary<TooltipType, GirlToolTipOpener>
	{
	}

	[SerializeField]
	private TooltipDictionary dictionary;

	private IDisposable disposable;

	public GirlToolTipOpener ShowOpener => dictionary[TooltipType.Show];

	public void Init(CharacterObjectConfig.TooltipSettingsDictionary settings)
	{
		foreach (KeyValuePair<TooltipType, ToolTipSettings> setting in settings)
		{
			dictionary[setting.Key].Init(setting.Value);
		}
		Enable();
	}

	internal void Init(CharacterAnimationSettings.ToolTipSetitngs toolTipSettings)
	{
		foreach (GirlToolTipOpener value in dictionary.Values)
		{
			UpdatePoistionAndPivot(value.Settings, toolTipSettings);
		}
		static void UpdatePoistionAndPivot(ToolTipSettings settings, CharacterAnimationSettings.ToolTipSetitngs concreteSettings)
		{
			settings.PivotPosition = concreteSettings.Pivot;
			settings.ToolTipPosition = concreteSettings.Position;
		}
	}

	public void Enable()
	{
		disposable = dictionary.Values.ToObservable().SelectMany((GirlToolTipOpener _opener) => from x in _opener.ReadyToActivate
			where x
			select x into _
			select _opener).Subscribe(TryTyOpen);
	}

	private void OnDisable()
	{
		disposable?.Dispose();
	}

	private void TryTyOpen(DependentToolTipOpener<ToolTipSettings, ToolTipGirlView> opener)
	{
		if (!(from o in dictionary.Values
			where o != opener
			select o.IsPlaying).Any((ReactiveProperty<bool> p) => p.Value))
		{
			dictionary[TooltipType.Idle].Deactivate();
			opener.Activate();
		}
	}
}
