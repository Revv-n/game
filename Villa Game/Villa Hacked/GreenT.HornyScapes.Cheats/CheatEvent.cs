using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes._HornyScapes._Scripts.Cheats;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatEvent : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button currency;

	[SerializeField]
	private Image currencyimage;

	[SerializeField]
	private Button target;

	[SerializeField]
	private Image targetImage;

	private EventSettingsProvider _eventSettingsProvider;

	private CalendarManager _calendarManager;

	private CalendarQueue _calendarQueue;

	private ICurrencyProcessor _currencyProcessor;

	private IDisposable _calendarInProgressStream;

	[Inject]
	private void InnerInit(CalendarQueue calendarQueue, ICurrencyProcessor currencyProcessor, EventSettingsProvider eventSettingsProvider, CalendarManager calendarManager)
	{
		_calendarManager = calendarManager;
		_calendarQueue = calendarQueue;
		_currencyProcessor = currencyProcessor;
		_eventSettingsProvider = eventSettingsProvider;
	}

	private void Awake()
	{
		currency.onClick.AddListener(AddEventCurrency);
		target.onClick.AddListener(AddEventExperience);
		_calendarInProgressStream = ObservableExtensions.Subscribe<CalendarModel>(Observable.SelectMany<CalendarModel, CalendarModel>(Observable.Where<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(), (Func<CalendarModel, bool>)delegate(CalendarModel model)
		{
			EventStructureType eventType = model.EventType;
			return eventType == EventStructureType.Event || eventType == EventStructureType.Mini;
		}), (Func<CalendarModel, IEnumerable<CalendarModel>>)((CalendarModel _) => _eventSettingsProvider.GetActiveModels(_calendarQueue.ToList()))), (Action<CalendarModel>)SetSprites);
	}

	private void SetSprites(CalendarModel calendarModel)
	{
		Event @event = _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
		if (@event != null)
		{
			currencyimage.sprite = @event.ViewSettings.Currency;
			targetImage.sprite = @event.ViewSettings.Target;
		}
	}

	private void AddEventCurrency()
	{
		AddCurrency(CurrencyType.Event);
	}

	private void AddEventExperience()
	{
		AddCurrency(CurrencyType.EventXP);
	}

	private void AddCurrency(CurrencyType type)
	{
		_currencyProcessor.TryChangeAmount(type, ReadValue());
	}

	private int ReadValue()
	{
		int result;
		int result2 = ((!int.TryParse(inputField.text, out result)) ? 1 : result);
		inputField.text = string.Empty;
		return result2;
	}

	[EditorButton]
	public void PrintCalendarCollection()
	{
		foreach (CalendarModel item in _calendarManager.Collection)
		{
			_ = item;
		}
	}

	[EditorButton]
	public void PrintCalendarQueue()
	{
		foreach (CalendarModel item in _calendarQueue)
		{
			_ = item;
		}
	}

	private void OnDestroy()
	{
		currency.onClick.RemoveAllListeners();
		target.onClick.RemoveAllListeners();
		_calendarInProgressStream.Dispose();
	}
}
