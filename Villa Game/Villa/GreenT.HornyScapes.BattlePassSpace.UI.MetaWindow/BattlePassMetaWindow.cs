using System;
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
		_calendarStream = _calendarQueue.OnCalendarActive(EventStructureType.BattlePass).Subscribe(Set);
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

	public void OpenFromEvent(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		_eventBattlePassViewer.Set(eventCalendarModel, battlePass);
		_eventButton.Set(eventCalendarModel, battlePass);
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
