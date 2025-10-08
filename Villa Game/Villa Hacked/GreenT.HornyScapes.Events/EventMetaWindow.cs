using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.UI;
using Merge;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventMetaWindow : Window
{
	[SerializeField]
	private EventButtonView eventButtonView;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private IDisposable _calendarStream;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	private CalendarQueue _calendarQueue;

	[Inject]
	private void InnerInit(CalendarQueue calendarQueue)
	{
		_calendarQueue = calendarQueue;
	}

	protected override void Awake()
	{
		Set(null);
		base.Awake();
		_calendarStream = ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarActive(EventStructureType.Event), (Action<CalendarModel>)Set);
	}

	public override void Open()
	{
		_disposables.Clear();
		base.Open();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Open(), (Action)starters.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			starters.InitClosers();
		}), (ICollection<IDisposable>)_disposables);
	}

	public override void Close()
	{
		_disposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Close(), (Action)base.Close), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}), (ICollection<IDisposable>)_disposables);
	}

	private void Set(CalendarModel calendarModel)
	{
		bool flag = calendarModel != null && calendarModel.EventType == EventStructureType.Event;
		eventButtonView.SetActive(flag);
		if (flag)
		{
			eventButtonView.Set(calendarModel);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_calendarStream?.Dispose();
	}
}
