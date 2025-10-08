using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class BattlePassMetaWindow : Window
{
	[SerializeField]
	private BattlePassButtonView _battlePassButtonView;

	[SerializeField]
	private EventBattlePassButton _eventButton;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private CalendarQueue _calendarQueue;

	private EventBattlePassViewer _eventBattlePassViewer;

	private IDisposable _calendarStream;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	private void Construct(CalendarQueue calendarQueue, EventBattlePassViewer eventBattlePassViewer)
	{
		_calendarQueue = calendarQueue;
		_eventBattlePassViewer = eventBattlePassViewer;
	}

	protected override void Awake()
	{
		Set(null);
		base.Awake();
		_calendarStream = ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarActive(EventStructureType.BattlePass), (Action<CalendarModel>)Set);
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

	public void OpenFromEvent(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		_eventBattlePassViewer.Set(eventCalendarModel, battlePass);
		_eventButton.Set(eventCalendarModel, battlePass);
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
		bool flag = calendarModel != null && calendarModel.EventType == EventStructureType.BattlePass;
		_battlePassButtonView.SetIsSoonState(!flag);
		if (flag)
		{
			_battlePassButtonView.Set(calendarModel);
		}
	}

	public Transform GetIconTransform()
	{
		return _battlePassButtonView.GetLevelHolder();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_calendarStream?.Dispose();
	}
}
