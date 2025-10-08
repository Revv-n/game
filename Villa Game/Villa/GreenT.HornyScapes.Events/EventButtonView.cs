using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventButtonView : MonoView<CalendarModel>
{
	[SerializeField]
	private EventButtonSubscription subscription;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private MonoTimer timerView;

	[SerializeField]
	private Sprite comingSoonSp;

	[SerializeField]
	private Sprite rewardSp;

	[SerializeField]
	private GameObject _description;

	[SerializeField]
	private List<StatableComponent> statableComponents = new List<StatableComponent>();

	private TimeHelper timeHelper;

	private IDisposable _calendarStateStream;

	private EventSettingsProvider _eventSettingsProvider;

	[Inject]
	public void Init(TimeHelper timeHelper, EventSettingsProvider provider)
	{
		this.timeHelper = timeHelper;
		_eventSettingsProvider = provider;
	}

	private void OnDestroy()
	{
		_calendarStateStream?.Dispose();
	}

	public override void Set(CalendarModel source)
	{
		base.Set(source);
		subscription.Set(source);
		_calendarStateStream?.Dispose();
		_calendarStateStream = source.CalendarState.Subscribe(SetState);
	}

	private void SetState(EntityStatus status)
	{
		Event @event = _eventSettingsProvider.GetEvent(base.Source.BalanceId);
		if (@event == null)
		{
			return;
		}
		switch (status)
		{
		case EntityStatus.Blocked:
			SetPreviewEvent();
			break;
		case EntityStatus.InProgress:
			SetStartEvent();
			break;
		case EntityStatus.Complete:
			if (@event.HasRewards())
			{
				SetRewardEvent();
				break;
			}
			base.Source.SetRewarded();
			base.gameObject.SetActive(value: false);
			break;
		case EntityStatus.Rewarded:
			base.gameObject.SetActive(value: false);
			break;
		default:
			throw new Exception().SendException($"{GetType().Name}: doesn't have behaviour for {status.ToString()} int = {status}");
		}
	}

	private void SetPreviewEvent()
	{
		ChangeState();
		icon.sprite = comingSoonSp;
		SetActivateDescription(isActive: true);
		SetTimer(base.Source.ComingSoonTimer);
	}

	private void SetStartEvent()
	{
		ChangeState();
		SetBankIcon();
		SetActivateDescription(isActive: false);
		SetTimer(base.Source.Duration);
	}

	private void SetRewardEvent()
	{
		ChangeState();
		SetActivateDescription(isActive: false);
		icon.sprite = rewardSp;
	}

	private void SetBankIcon()
	{
		Event @event = _eventSettingsProvider.GetEvent(base.Source.BalanceId);
		if (@event != null)
		{
			icon.sprite = @event.ViewSettings.ButtonSp;
		}
	}

	private void SetActivateDescription(bool isActive)
	{
		_description.SetActive(isActive);
	}

	private void SetTimer(GenericTimer timer)
	{
		timerView.Init(timer, timeHelper.UseCombineFormat);
	}

	private void ChangeState()
	{
		base.gameObject.SetActive(value: true);
		int stateNumber = base.Source.CalendarState.Value.ConvertToInt();
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(stateNumber);
		}
	}
}
