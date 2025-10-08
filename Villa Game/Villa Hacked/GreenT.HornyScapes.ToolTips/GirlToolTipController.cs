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
		disposable = ObservableExtensions.Subscribe<GirlToolTipOpener>(Observable.SelectMany<GirlToolTipOpener, GirlToolTipOpener>(Observable.ToObservable<GirlToolTipOpener>((IEnumerable<GirlToolTipOpener>)dictionary.Values), (Func<GirlToolTipOpener, IObservable<GirlToolTipOpener>>)((GirlToolTipOpener _opener) => Observable.Select<bool, GirlToolTipOpener>(Observable.Where<bool>((IObservable<bool>)_opener.ReadyToActivate, (Func<bool, bool>)((bool x) => x)), (Func<bool, GirlToolTipOpener>)((bool _) => _opener)))), (Action<GirlToolTipOpener>)TryTyOpen);
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
