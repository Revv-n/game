using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		_calendarQueue = calendarQueue;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_disposables = new CompositeDisposable();
	}

	private void Awake()
	{
		IObservable<bool> observable = Observable.SelectMany<MiniEvent, bool>(Observable.ToObservable<MiniEvent>(_miniEventSettingsProvider.Collection), (Func<MiniEvent, IObservable<bool>>)OnNewContentAvailable);
		IObservable<bool> observable2 = Observable.SelectMany<MiniEvent, bool>(_miniEventSettingsProvider.OnNew, (Func<MiniEvent, IObservable<bool>>)OnNewContentAvailable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Merge<bool>(observable, new IObservable<bool>[1] { observable2 }), (Action<bool>)delegate
		{
			OnMiniEventStateChange();
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Mini), (Action<CalendarModel>)delegate
		{
			OnMiniEventStateChange();
		}), (ICollection<IDisposable>)_disposables);
		static IObservable<bool> OnNewContentAvailable(MiniEvent minievent)
		{
			return Observable.Select<bool, bool>(Observable.Skip<bool>((IObservable<bool>)minievent.IsAnyContentAvailable, 1), (Func<bool, bool>)((bool _) => _));
		}
	}

	private void OnDestroy()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
		CompositeDisposable disposables2 = _disposables;
		if (disposables2 != null)
		{
			disposables2.Dispose();
		}
	}

	private void OnMiniEventStateChange()
	{
		_buttonRoot.SetActive(_miniEventSettingsProvider.Collection.Any((MiniEvent minievent) => minievent.IsAnyContentAvailable.Value) && _calendarQueue.HasInProgressCalendar(EventStructureType.Mini));
	}
}
