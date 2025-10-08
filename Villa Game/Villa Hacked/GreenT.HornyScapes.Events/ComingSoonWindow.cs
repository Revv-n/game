using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class ComingSoonWindow : PopupWindow
{
	[SerializeField]
	private MonoTimer timerView;

	[SerializeField]
	private LocalizedTextMeshPro data;

	private IDisposable changeStateStream;

	private int id = 1001;

	private TimeHelper timeHelper;

	private const string LocalePrefix = "ui.day_";

	[Inject]
	public void Init(TimeHelper timeHelper)
	{
		this.timeHelper = timeHelper;
	}

	public void Set(CalendarModel calendarModel)
	{
		changeStateStream?.Dispose();
		changeStateStream = ObservableExtensions.Subscribe<EntityStatus>(Observable.Where<EntityStatus>(Observable.Take<EntityStatus>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)calendarModel.CalendarState, (Func<EntityStatus, bool>)((EntityStatus _state) => _state != EntityStatus.Blocked)), 1), (Func<EntityStatus, bool>)((EntityStatus _) => IsOpened)), (Action<EntityStatus>)OnChangeSourceState);
		SetTimer(calendarModel.ComingSoonTimer);
		if (calendarModel is PeriodicCalendar date)
		{
			SetDate(date);
		}
	}

	private void OnChangeSourceState(EntityStatus state)
	{
		Close();
	}

	private void SetTimer(GenericTimer timer)
	{
		timerView.Init(timer, timeHelper.UseCombineFormat);
	}

	private void SetDate(PeriodicCalendar periodicCalendar)
	{
		string text = "ui.day_" + periodicCalendar.ComingSoonDate.DayOfWeek;
		string text2 = "ui.day_" + periodicCalendar.EndDate.DayOfWeek;
		data.SetArguments(text, text2);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		changeStateStream?.Dispose();
	}
}
