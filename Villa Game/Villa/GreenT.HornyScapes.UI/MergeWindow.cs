using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class MergeWindow : Window
{
	[Inject]
	private ICameraChanger cameraChanger;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private GameObject field;

	[SerializeField]
	private Image eventBankIcon;

	[SerializeField]
	private List<GameObject> controllers;

	[SerializeField]
	private AnimationSetOpenCloseController animations;

	private EventSettingsProvider _eventSettingsProvider;

	private CalendarQueue _calendarQueue;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	public void Init(ContentSelectorGroup contentSelectorGroup, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider)
	{
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
		cameraChanger.AddCameraToWindow(this, camera);
		camera.enabled = false;
		animations.InitOpeners();
		SetState(state: false);
	}

	public override void Open()
	{
		_disposables.Clear();
		ChangeBankButton();
		base.Open();
		SetState(state: true);
		animations.Open().DoOnCancel(animations.InitClosers).Subscribe(delegate
		{
			animations.InitClosers();
		})
			.AddTo(_disposables);
	}

	public override void Close()
	{
		_disposables.Clear();
		animations.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			InstantClose();
		})
			.AddTo(_disposables);
	}

	private void InstantClose()
	{
		SetState(state: false);
		base.Close();
	}

	private void ChangeBankButton()
	{
		if (!_calendarQueue.HasActiveCalendar())
		{
			return;
		}
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null)
		{
			GreenT.HornyScapes.Events.Event @event = _eventSettingsProvider.GetEvent(activeCalendar.BalanceId);
			if (@event != null)
			{
				eventBankIcon.sprite = @event.Bundle.BankButtonSp;
			}
		}
	}

	private void SetState(bool state)
	{
		field.SetActive(state);
		base.Canvas.enabled = state;
		foreach (GameObject item in controllers.Where((GameObject _c) => _c != null))
		{
			item.SetActive(state);
		}
	}

	protected override void OnDestroy()
	{
		_disposables.Dispose();
		cameraChanger.RemovewCameraByWindow(this);
		base.OnDestroy();
	}
}
