using System;
using System.Linq;
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
		_calendarDisposable = _calendarQueue.OnCalendarStateChange(EventStructureType.Mini).Subscribe(delegate
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
		IObservable<bool> first = _miniEventTabsManager.Collection.ToObservable().SelectMany((MiniEventActivityTab tab) => tab.IsAnyActionAvailable).Merge();
		IObservable<bool> first2 = _minieventSettingsProvider.Collection.ToObservable().SelectMany((MiniEvent minievent) => minievent.IsSpawned);
		IObservable<bool> observable = _minieventSettingsProvider.OnNew.SelectMany((MiniEvent minievent) => minievent.IsSpawned);
		IObservable<bool> first3 = first2.Merge(observable);
		IObservable<bool> first4 = _minieventSettingsProvider.Collection.ToObservable().SelectMany((MiniEvent minievent) => minievent.WasFirstTimeSeen);
		IObservable<bool> observable2 = _minieventSettingsProvider.OnNew.SelectMany((MiniEvent minievent) => minievent.WasFirstTimeSeen);
		IObservable<bool> observable3 = first4.Merge(observable2);
		IObservable<bool> observable4 = first3.Merge(observable3);
		IObservable<bool> source = first.Merge(observable4);
		_disposable = source.Subscribe(delegate
		{
			bool flag = _minieventSettingsProvider.Collection.Any((MiniEvent minievent) => !minievent.WasFirstTimeSeen.Value && minievent.IsSpawned.Value);
			bool flag2 = _miniEventTabsManager.Collection.Any((MiniEventActivityTab tab) => tab.IsAnyActionAvailable.Value);
			_pimp.SetActive(flag || flag2);
		});
	}
}
