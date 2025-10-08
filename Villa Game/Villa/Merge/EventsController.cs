using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Merge.Core.Events;
using Merge.Core.Masters;
using Merge.Core.Tasks;
using MISC.Resolution;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class EventsController : Controller<EventsController>, IDataController, IMasterController
{
	[Serializable]
	public class Element
	{
		public string description;

		public RectTransformResolutionAdapter adapter;

		public List<RectTransformResolutionAdapter.RTResolutionPreset> presets;

		public void Apply()
		{
			adapter.OverridePresets(presets);
		}
	}

	[Serializable]
	public class GeneralData : Data
	{
		[SerializeField]
		private int currentEventKey = 1;

		[SerializeField]
		private int eventStatus;

		[SerializeField]
		private int currency;

		[SerializeField]
		private int currencyTotalInEvent;

		[SerializeField]
		private RefTimer timer;

		[SerializeField]
		private int refreshShopCount;

		[SerializeField]
		private bool eventRequirements;

		[SerializeField]
		private float timeInCurrentEvent;

		[SerializeField]
		private int entrancesInCurrentEvent;

		public EventStatus EventStatus
		{
			get
			{
				return (EventStatus)eventStatus;
			}
			set
			{
				eventStatus = (int)value;
			}
		}

		public int Currency
		{
			get
			{
				return currency;
			}
			set
			{
				currency = value;
			}
		}

		public RefTimer Timer
		{
			get
			{
				return timer;
			}
			set
			{
				timer = value;
			}
		}

		public int RefreshShopCount
		{
			get
			{
				return refreshShopCount;
			}
			set
			{
				refreshShopCount = value;
			}
		}

		public bool EventRequirements
		{
			get
			{
				return eventRequirements;
			}
			set
			{
				eventRequirements = value;
			}
		}

		public float TimeInCurrentEvent
		{
			get
			{
				return timeInCurrentEvent;
			}
			set
			{
				timeInCurrentEvent = value;
			}
		}

		public int EntrancesInCurrentEvent
		{
			get
			{
				return entrancesInCurrentEvent;
			}
			set
			{
				entrancesInCurrentEvent = value;
			}
		}

		public int CurrentEventKey
		{
			get
			{
				return currentEventKey;
			}
			set
			{
				currentEventKey = value;
			}
		}

		public int CurrencyTotalInEvent
		{
			get
			{
				return currencyTotalInEvent;
			}
			set
			{
				currencyTotalInEvent = value;
			}
		}

		public void Clear()
		{
			EventStatus = EventStatus.inactive;
			Currency = 0;
			Timer = null;
			RefreshShopCount = 0;
			EntrancesInCurrentEvent = 0;
			TimeInCurrentEvent = 0f;
			CurrencyTotalInEvent = 0;
			Debug.Log("Clear Event");
		}
	}

	private const int SEND_EVENT_EVERY_X_POINTS = 100;

	private const bool Logs = false;

	private static bool FirstShowed;

	private static bool ForceCompleteFlag;

	private bool testEvent;

	[SerializeField]
	private EventMainButton eventButton;

	[SerializeField]
	private EventWindow eventWindow;

	[SerializeField]
	private Text eventScoreText;

	[SerializeField]
	private EventConfigScriptableObject eventConfig;

	private GeneralData data;

	private DateTime coreEventSessionStartTime;

	private TweenTimer timer;

	private bool dialogBlock;

	private bool needShowWindow;

	[SerializeField]
	private List<Element> elements;

	public string CurrentEvent => "event0";

	public int EventWaitDuration { get; } = 86400;


	public bool EventAvailable
	{
		get
		{
			if (data != null)
			{
				if (CurrentEventStatus != EventStatus.inProgress && CurrentEventStatus != EventStatus.available)
				{
					return CurrentEventStatus == EventStatus.completed;
				}
				return true;
			}
			return false;
		}
	}

	public bool EventRequirements
	{
		get
		{
			return data.EventRequirements;
		}
		private set
		{
			data.EventRequirements = value;
		}
	}

	public bool IsEventShopAwailable
	{
		get
		{
			if (CurrentEventStatus != EventStatus.inProgress)
			{
				return CurrentEventStatus == EventStatus.completed;
			}
			return true;
		}
	}

	public EventStatus CurrentEventStatus
	{
		get
		{
			if (data != null)
			{
				return data.EventStatus;
			}
			Debug.LogError("EventsController: Не загружены данные!");
			return EventStatus.inactive;
		}
		private set
		{
			if (data != null)
			{
				data.EventStatus = value;
			}
		}
	}

	public bool ShowTimer => timer != null;

	public int RefreshShopCount => data.RefreshShopCount;

	public static PlayType CurrentPlayType
	{
		get
		{
			return (PlayType)PlayerPrefs.GetInt("EventsController_CurrentPlayType", 0);
		}
		set
		{
			PlayerPrefs.SetInt("EventsController_CurrentPlayType", (int)value);
			PlayerPrefs.Save();
		}
	}

	public static bool IsStoryType => CurrentPlayType == PlayType.story;

	public static bool IsEventsType => CurrentPlayType == PlayType.events;

	public event Action<int, int> BeforeChangeValue;

	public event Action<int> OnChangeValue;

	public void Load(Data baseData)
	{
		data = (baseData as GeneralData) ?? new GeneralData();
		data.EventRequirements = true;
		SetStory();
		InitState();
	}

	private void InitState()
	{
		switch (CurrentEventStatus)
		{
		case EventStatus.inactive:
			AvaliableEvent();
			break;
		case EventStatus.inProgress:
			if (data.Timer != null && data.Timer.IsCompleted)
			{
				DateTime dateTime = data.Timer.EndTime.AddSeconds(1.0);
				if (TimeMaster.Now < dateTime)
				{
					CompleteEvent();
				}
				else
				{
					OffEvent();
				}
			}
			break;
		case EventStatus.completed:
			if (data.Timer != null && data.Timer.IsCompleted)
			{
				OffEvent();
			}
			break;
		case EventStatus.available:
			break;
		}
	}

	public override void Init()
	{
		base.Init();
		eventButton.AddCallback(ShowWindow);
		ApplyResolutionAdapters();
		ValidateButtonState();
		if (EventRequirements || testEvent)
		{
			UpdateCurrencyIndicator();
			InitTimer();
			if (ForceCompleteFlag)
			{
				ForceCompleteFlag = false;
				ShowWindow();
			}
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (data == null || data.Timer == null)
		{
			return;
		}
		if (focus)
		{
			coreEventSessionStartTime = TimeMaster.Now;
			if (data.Timer.IsCompleted)
			{
				CompleteEvent();
			}
		}
		else
		{
			float num = (float)(TimeMaster.Now - coreEventSessionStartTime).TotalSeconds;
			data.TimeInCurrentEvent += num;
		}
	}

	private void ValidateButtonState()
	{
		bool flag = true;
		if (flag)
		{
			flag = EventRequirements;
		}
		if (eventButton.gameObject.activeSelf != flag)
		{
			eventButton.gameObject.SetActive(flag);
		}
		eventButton.SetTimeActive(CurrentEventStatus == EventStatus.inProgress);
	}

	public Data GetSave()
	{
		float num = (float)(TimeMaster.Now - coreEventSessionStartTime).TotalSeconds;
		data.TimeInCurrentEvent += num;
		Debug.Log($"Time spend in event {num}, Tolal: {data.TimeInCurrentEvent}");
		coreEventSessionStartTime = TimeMaster.Now;
		return data;
	}

	private void AtTaskCompleted(TaskKey obj)
	{
		if (obj.Room > 0 && !EventRequirements)
		{
			ApplyEventRequirements();
		}
	}

	public void ApplyEventRequirements()
	{
		if (!EventRequirements)
		{
			EventRequirements = true;
			ValidateButtonState();
		}
	}

	[ContextMenu("ApplyResolutionAdapters")]
	private void ApplyResolutionAdapters()
	{
		if (CurrentPlayType == PlayType.story)
		{
			return;
		}
		foreach (Element element in elements)
		{
			element.Apply();
		}
	}

	private void InitTimer()
	{
		if (data.Timer != null && EventAvailable)
		{
			if (timer != null && !timer.Timer.IsCompleted)
			{
				timer.Kill();
			}
			if (!data.Timer.IsCompleted)
			{
				timer = new TweenTimer(data.Timer, AtTimerComplete, AtTimerTick);
			}
		}
	}

	private void AtTimerTick(TimerStatus status)
	{
		eventButton.SetTimerStatus(status);
	}

	private void AtTimerComplete()
	{
		if (CurrentEventStatus != EventStatus.inProgress)
		{
			timer?.Kill();
		}
		if (CurrentEventStatus == EventStatus.inactive)
		{
			AvaliableEvent();
		}
		else if (CurrentEventStatus == EventStatus.inProgress)
		{
			CompleteEvent();
		}
		else if (CurrentEventStatus == EventStatus.completed)
		{
			OffEvent();
		}
	}

	public void SetEvent()
	{
		if (CurrentEventStatus != EventStatus.inProgress)
		{
			CurrentEventStatus = EventStatus.inProgress;
		}
		CurrentPlayType = PlayType.events;
	}

	public void SetStory()
	{
		CurrentPlayType = PlayType.story;
	}

	public void ShowWindow()
	{
		if (CurrentEventStatus != EventStatus.inProgress || CurrentPlayType == PlayType.events)
		{
			if (!FirstShowed)
			{
				FirstShowed = true;
			}
			eventWindow.SetData(eventConfig.GetConfig(CurrentEventStatus), CurrentEventStatus).Show();
		}
		else
		{
			StartEvent();
		}
	}

	public void StartEvent()
	{
		SetEvent();
	}

	private void AvaliableEvent()
	{
		CurrentEventStatus = EventStatus.available;
	}

	private void CompleteEvent()
	{
		CurrentEventStatus = EventStatus.completed;
		InitTimer();
	}

	private void OffEvent()
	{
		Debug.Log($"Time spend in event {data.TimeInCurrentEvent}");
		data.Clear();
		Pocket.ClearEventData();
		Debug.Log("EVENT DATA CLEARED");
		AvaliableEvent();
	}

	public void OnRefreshRewardShop()
	{
		data.RefreshShopCount++;
	}

	public void LinkControllers(IList<BaseController> controllers)
	{
		(from x in controllers
			where x is IGameTypeListener
			select x as IGameTypeListener).ToList().ForEach(delegate(IGameTypeListener x)
		{
			x.SetGameType(CurrentPlayType);
		});
	}

	private void AtValueSet(int value, float delay = 0f)
	{
		int currency = data.Currency;
		data.Currency = value;
		if (value > currency)
		{
			data.CurrencyTotalInEvent += value - currency;
		}
		if (delay == 0f)
		{
			UpdateCurrencyIndicator();
		}
		else
		{
			DOVirtual.DelayedCall(delay, UpdateCurrencyIndicator);
		}
	}

	private void UpdateCurrencyIndicator()
	{
		if (!(eventScoreText == null) && data != null)
		{
			eventScoreText.text = data.Currency.ToString();
		}
	}
}
