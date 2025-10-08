using System;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventIconIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject _pimp;

	private MiniEventTabsManager _miniEventTabsManager;

	private MiniEventSettingsProvider _minieventSettingsProvider;

	private CalendarQueue _calendarQueue;

	private IDisposable _disposable;

	private IDisposable _calendarDisposable;

	[Inject]
	private void Init(MiniEventSettingsProvider minieventSettingsProvider, MiniEventTabsManager miniEventTabsManager, CalendarQueue calendarQueue)
	{
		_minieventSettingsProvider = minieventSettingsProvider;
		_miniEventTabsManager = miniEventTabsManager;
		_calendarQueue = calendarQueue;
	}

	private void Awake()
	{
		_calendarDisposable = ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Mini), (Action<CalendarModel>)delegate
		{
			TryStartTrack();
		});
	}

	private void OnDestroy()
	{
		_disposable?.Dispose();
		_calendarDisposable?.Dispose();
	}

	private void TryStartTrack()
	{
		if (!_calendarQueue.HasInProgressCalendar(EventStructureType.Mini))
		{
			_disposable?.Dispose();
			return;
		}
		IObservable<bool> observable = Observable.Merge<bool>(Observable.SelectMany<MiniEventActivityTab, bool>(Observable.ToObservable<MiniEventActivityTab>(_miniEventTabsManager.Collection), (Func<MiniEventActivityTab, IObservable<bool>>)((MiniEventActivityTab tab) => (IObservable<bool>)tab.IsAnyActionAvailable)), Array.Empty<IObservable<bool>>());
		IObservable<bool> observable2 = Observable.SelectMany<MiniEvent, bool>(Observable.ToObservable<MiniEvent>(_minieventSettingsProvider.Collection), (Func<MiniEvent, IObservable<bool>>)((MiniEvent minievent) => (IObservable<bool>)minievent.IsSpawned));
		IObservable<bool> observable3 = Observable.SelectMany<MiniEvent, bool>(_minieventSettingsProvider.OnNew, (Func<MiniEvent, IObservable<bool>>)((MiniEvent minievent) => (IObservable<bool>)minievent.IsSpawned));
		IObservable<bool> observable4 = Observable.Merge<bool>(observable2, new IObservable<bool>[1] { observable3 });
		IObservable<bool> observable5 = Observable.SelectMany<MiniEvent, bool>(Observable.ToObservable<MiniEvent>(_minieventSettingsProvider.Collection), (Func<MiniEvent, IObservable<bool>>)((MiniEvent minievent) => (IObservable<bool>)minievent.WasFirstTimeSeen));
		IObservable<bool> observable6 = Observable.SelectMany<MiniEvent, bool>(_minieventSettingsProvider.OnNew, (Func<MiniEvent, IObservable<bool>>)((MiniEvent minievent) => (IObservable<bool>)minievent.WasFirstTimeSeen));
		IObservable<bool> observable7 = Observable.Merge<bool>(observable5, new IObservable<bool>[1] { observable6 });
		IObservable<bool> observable8 = Observable.Merge<bool>(observable4, new IObservable<bool>[1] { observable7 });
		IObservable<bool> observable9 = Observable.Merge<bool>(observable, new IObservable<bool>[1] { observable8 });
		_disposable = ObservableExtensions.Subscribe<bool>(observable9, (Action<bool>)delegate
		{
			bool flag = _minieventSettingsProvider.Collection.Any((MiniEvent minievent) => !minievent.WasFirstTimeSeen.Value && minievent.IsSpawned.Value);
			bool flag2 = _miniEventTabsManager.Collection.Any((MiniEventActivityTab tab) => tab.IsAnyActionAvailable.Value);
			_pimp.SetActive(flag || flag2);
		});
	}
}
