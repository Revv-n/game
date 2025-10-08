using System;
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
		_calendarStream = _calendarQueue.OnCalendarActive(EventStructureType.Event).Subscribe(Set);
	}

	public override void Open()
	{
		_disposables.Clear();
		base.Open();
		starters.Open().DoOnCancel(starters.InitClosers).Subscribe(delegate
		{
			starters.InitClosers();
		})
			.AddTo(_disposables);
	}

	public override void Close()
	{
		_disposables.Clear();
		starters.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			base.Close();
		})
			.AddTo(_disposables);
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
