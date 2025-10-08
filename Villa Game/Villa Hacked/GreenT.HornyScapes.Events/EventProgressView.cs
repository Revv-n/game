using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventProgressView : EventView
{
	public const int START_EVENT = 0;

	public const int PROGRESS_EVENT = 1;

	public const int COMPLETE_EVENT = 2;

	[SerializeField]
	protected List<StatableComponent> statableComponents = new List<StatableComponent>();

	[SerializeField]
	protected Button rewardButton;

	[SerializeField]
	private Image headerBackground;

	[SerializeField]
	private Window eventProgressWindow;

	private IDisposable _buttonClickStream;

	private EventRewardCardManager _cardManager;

	private int _currentState;

	[Inject]
	private void Constructor(EventRewardCardManager cardManager)
	{
		_cardManager = cardManager;
	}

	public override void Initialize()
	{
		base.Initialize();
		_buttonClickStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(rewardButton), (Action<Unit>)delegate
		{
			SetRewarded();
		});
	}

	public override void Set(CalendarModel calendarModel)
	{
		base.Set(calendarModel);
		Event @event = _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
		if (@event != null)
		{
			ViewSettings viewSettings = @event.ViewSettings;
			background.sprite = viewSettings.AnnouncementBackground;
			headerBackground.sprite = viewSettings.AnnouncementTitleBackground;
			if (base.Source != null)
			{
				ShowRewardCards(base.Source);
			}
		}
	}

	protected override Sprite GetFocusGirl(ICharacter character)
	{
		return character.BankImages.Big;
	}

	private void ShowRewardCards(CalendarModel calendarModel)
	{
		Event @event = _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
		if (@event == null)
		{
			return;
		}
		bool flag = calendarModel.CalendarState.Value == EntityStatus.Complete;
		IReadOnlyList<EventReward> readOnlyList = @event.GetEventRewards();
		if (flag)
		{
			readOnlyList = readOnlyList.TakeWhile((EventReward _reward) => _reward.State.Value == EntityStatus.Rewarded).ToList();
		}
		_cardManager.HideAll();
		foreach (EventReward item in readOnlyList)
		{
			EventRewardCard cardToCompleteView = _cardManager.Display(item);
			if (flag)
			{
				SetCardToCompleteView(cardToCompleteView);
			}
		}
	}

	private void OnEnable()
	{
		if (base.Source == null || base.Source.CalendarState.Value != EntityStatus.InProgress)
		{
			return;
		}
		IEnumerable<LinkedContent> allRewardsContent = base.Source.RewardHolder.GetAllRewardsContent();
		foreach (EventRewardCard view in _cardManager.Views)
		{
			if (!view.IsActive() && allRewardsContent.Any((LinkedContent reward) => view.Source.Content == reward))
			{
				view.Display(display: true);
			}
			view.SetWindowDependenciesState(1);
		}
	}

	private void SetRewarded()
	{
		base.Source.SetRewarded();
		eventProgressWindow.Close();
	}

	public virtual void SetViewState(int state)
	{
		_currentState = state;
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(state);
		}
		foreach (EventRewardCard visibleView in _cardManager.VisibleViews)
		{
			visibleView.SetWindowDependenciesState(state);
		}
		if (state != 2)
		{
			return;
		}
		foreach (EventRewardCard visibleView2 in _cardManager.VisibleViews)
		{
			SetCardToCompleteView(visibleView2);
		}
	}

	private void SetCardToCompleteView(EventRewardCard card)
	{
		bool flag = card.Source.State.Value == EntityStatus.Rewarded;
		if (card.IsActive() != flag)
		{
			card.Display(flag);
		}
		card.SetViewState(card.GetStateNumber(EntityStatus.InProgress));
	}

	public override void Dispose()
	{
		base.Dispose();
		_buttonClickStream?.Dispose();
	}
}
