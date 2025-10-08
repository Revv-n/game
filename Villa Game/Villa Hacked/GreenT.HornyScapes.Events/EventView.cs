using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public abstract class EventView : MonoView<CalendarModel>, IDisposable, IInitializable
{
	[SerializeField]
	protected LocalizedTextMeshPro title;

	[SerializeField]
	protected LocalizedTextMeshPro titleShadow;

	[SerializeField]
	protected LocalizedTextMeshPro description;

	[SerializeField]
	protected List<StatableComponent> rarityStatableComponents = new List<StatableComponent>();

	[SerializeField]
	protected MonoTimer[] timerViews;

	[SerializeField]
	protected GameObject focusCard;

	[SerializeField]
	protected GameObject plus;

	[SerializeField]
	protected Image girlImage;

	[SerializeField]
	protected Image focusGirlImage;

	[SerializeField]
	protected LocalizedTextMeshPro focusGirlName;

	[SerializeField]
	protected Image background;

	private IDisposable _calendarTrackStream;

	private CalendarQueue _calendarQueue;

	private CharacterManager _characterManager;

	private TimeHelper _timeHelper;

	protected EventSettingsProvider _eventSettingsProvider;

	[Inject]
	private void InnerInit(CalendarQueue calendarQueue, CharacterManager characterManager, TimeHelper timeHelper, EventSettingsProvider provider)
	{
		_calendarQueue = calendarQueue;
		_characterManager = characterManager;
		_timeHelper = timeHelper;
		_eventSettingsProvider = provider;
	}

	public virtual void Initialize()
	{
		ActiveCalendarSubscribe();
	}

	private void ActiveCalendarSubscribe()
	{
		_calendarTrackStream = ObservableExtensions.Subscribe<CalendarModel>(Observable.Where<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Event), (Func<CalendarModel, bool>)CalendarIsActive), (Action<CalendarModel>)Set);
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar != null)
		{
			Set(activeCalendar);
		}
	}

	public override void Set(CalendarModel calendarModel)
	{
		base.Set(calendarModel);
		Event @event = _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
		if (@event != null)
		{
			EventBundleData bundle = @event.Bundle;
			InitTexts(bundle);
			SetSprites(@event);
			SetFocusGirl(@event);
			SetTimers(calendarModel);
			SetRarity(@event);
		}
	}

	private void SetRarity(Event eventSettings)
	{
		if (eventSettings.FocusID == -1)
		{
			return;
		}
		ICharacter girl = GetGirl(eventSettings);
		foreach (StatableComponent rarityStatableComponent in rarityStatableComponents)
		{
			rarityStatableComponent.Set((int)girl.Rarity);
		}
	}

	private ICharacter GetGirl(Event eventSettings)
	{
		return _characterManager.Get(eventSettings.FocusID);
	}

	private void SetTimers(CalendarModel calendarModel)
	{
		MonoTimer[] array = timerViews;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init(calendarModel.Duration, _timeHelper.UseCombineFormat);
		}
	}

	private void SetSprites(Event eventSettings)
	{
		girlImage.sprite = eventSettings.ViewSettings.ProgressGirl;
	}

	private void SetFocusGirl(Event eventSettings)
	{
		bool flag = 0 < eventSettings.FocusID;
		focusCard.SetActive(flag);
		if (plus != null)
		{
			plus.SetActive(flag);
		}
		if (flag)
		{
			ICharacter girl = GetGirl(eventSettings);
			focusGirlImage.sprite = GetFocusGirl(girl);
			focusGirlName.Init(girl.NameKey);
		}
	}

	protected abstract Sprite GetFocusGirl(ICharacter character);

	private void InitTexts(EventBundleData bundle)
	{
		title.Init(bundle.TitleKeyLoc);
		titleShadow.Init(bundle.TitleKeyLoc);
		description.Init(bundle.DescriptionKeyLoc);
	}

	private bool CalendarIsActive(CalendarModel calendarModel)
	{
		if (calendarModel.CalendarState.Value != EntityStatus.Blocked)
		{
			return calendarModel.CalendarState.Value != EntityStatus.Rewarded;
		}
		return false;
	}

	public virtual void Dispose()
	{
		_calendarTrackStream?.Dispose();
	}
}
