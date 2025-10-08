using System;
using System.Linq;
using GreenT.HornyScapes.Events;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventsButtonView : MonoView
{
	[SerializeField]
	private GameObject _buttonRoot;

	private MiniEventSettingsProvider _miniEventSettingsProvider;

	private CalendarQueue _calendarQueue;

	private CompositeDisposable _disposables;

	[Inject]
	private void Init(CalendarQueue calendarQueue, MiniEventSettingsProvider miniEventSettingsProvider)
	{
		_calendarQueue = calendarQueue;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_disposables = new CompositeDisposable();
	}

	private void Awake()
	{
		IObservable<bool> first = _miniEventSettingsProvider.Collection.ToObservable().SelectMany((Func<MiniEvent, IObservable<bool>>)OnNewContentAvailable);
		IObservable<bool> observable = _miniEventSettingsProvider.OnNew.SelectMany((Func<MiniEvent, IObservable<bool>>)OnNewContentAvailable);
		first.Merge(observable).Subscribe(delegate
		{
			OnMiniEventStateChange();
		}).AddTo(_disposables);
		_calendarQueue.OnCalendarStateChange(EventStructureType.Mini).Subscribe(delegate
		{
			OnMiniEventStateChange();
		}).AddTo(_disposables);
		static IObservable<bool> OnNewContentAvailable(MiniEvent minievent)
		{
			return from _ in minievent.IsAnyContentAvailable.Skip(1)
				select (_);
		}
	}

	private void OnDestroy()
	{
		_disposables?.Clear();
		_disposables?.Dispose();
	}

	private void OnMiniEventStateChange()
	{
		_buttonRoot.SetActive(_miniEventSettingsProvider.Collection.Any((MiniEvent minievent) => minievent.IsAnyContentAvailable.Value) && _calendarQueue.HasInProgressCalendar(EventStructureType.Mini));
	}
}
