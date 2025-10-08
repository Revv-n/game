using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.UI;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventProgressBar : MonoBehaviour
{
	[SerializeField]
	private AnimatedProgress progressBar;

	private IReadOnlyReactiveProperty<int> eventXP;

	private int previousTarget;

	private CompositeDisposable currencyChangeStream = new CompositeDisposable();

	private EventRewardTracker eventRewardTracker;

	private ICurrencyProcessor currencyProcessor;

	private CalendarQueue calendarQueue;

	[Inject]
	private void InnerInit(EventRewardTracker eventRewardTracker, ICurrencyProcessor currencyProcessor, CalendarQueue calendarQueue)
	{
		this.currencyProcessor = currencyProcessor;
		this.eventRewardTracker = eventRewardTracker;
		this.calendarQueue = calendarQueue;
	}

	private void OnDestroy()
	{
		currencyChangeStream?.Dispose();
	}

	private void Start()
	{
		eventRewardTracker.Cleared.Subscribe(delegate
		{
			CheckRewardTracker();
		}).AddTo(this);
		if (!currencyProcessor.TryGetCountReactiveProperty(CurrencyType.EventXP, out eventXP))
		{
			Debug.LogError(new Exception("EventXP is null"));
		}
		else
		{
			CheckRewardTracker();
		}
	}

	private void CheckRewardTracker()
	{
		ReactiveProperty<BaseReward> target = eventRewardTracker.Target;
		CalendarModel activeCalendar = calendarQueue.GetActiveCalendar(EventStructureType.Event);
		int num = 0;
		if (activeCalendar != null)
		{
			num = ((EventMapper)activeCalendar.EventMapper).bp_id;
		}
		if (target.Value != null && num == 0)
		{
			target.CombineLatest(eventXP, (BaseReward _borders, int _eventXP) => (current: _eventXP, target: _borders?.Target ?? 0, previous: (_borders?.PrevReward?.Target).GetValueOrDefault())).Subscribe(UpdateProgressSlider).AddTo(currencyChangeStream);
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void UpdateProgressSlider((int current, int target, int previous) barsettings)
	{
		progressBar.AnimateFromCurrent(barsettings.current, barsettings.target, barsettings.previous);
	}
}
