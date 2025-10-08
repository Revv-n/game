using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Views;
using GreenT.HornyScapes.Dates.Windows;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Dates.Services;

public class DateController : IInitializable, IDisposable
{
	private IDisposable _startStream;

	private DateLoadingWindow _loadingWindow;

	private readonly IWindowsManager _windowsManager;

	private readonly DateLoadService _dateLoadService;

	private readonly DateFlowController _dateFlowController;

	private readonly DateUnloadService _dateUnloadService;

	private readonly EyeView _eyeView;

	private readonly DateSoundController _dateSoundController;

	private readonly float _delay = 0.5f;

	private readonly Subject<Date> _dateStartRequested = new Subject<Date>();

	public DateController(IWindowsManager windowsManager, DateFlowController dateFlowController, DateLoadService dateLoadService, DateUnloadService dateUnloadService, EyeView eyeView, DateSoundController dateSoundController)
	{
		_windowsManager = windowsManager;
		_dateLoadService = dateLoadService;
		_dateFlowController = dateFlowController;
		_dateUnloadService = dateUnloadService;
		_eyeView = eyeView;
		_dateSoundController = dateSoundController;
	}

	public void StartDate(Date date)
	{
		_dateStartRequested.OnNext(date);
		_dateSoundController.SetActiveMusic(isActive: false);
	}

	public void Initialize()
	{
		_startStream = ObservableExtensions.Subscribe<Date>(Observable.Switch<Date>(Observable.Select<Date, IObservable<Date>>((IObservable<Date>)_dateStartRequested, (Func<Date, IObservable<Date>>)delegate(Date date)
		{
			IObservable<Date> observable = Observable.RefCount<Date>(Observable.Publish<Date>(Observable.Select<Unit, Date>(_dateLoadService.LoadDate(date), (Func<Unit, Date>)((Unit _) => date))));
			ObservableExtensions.Subscribe<long>(Observable.Do<long>(Observable.TakeUntil<long, Date>(Observable.Timer(TimeSpan.FromSeconds(_delay)), observable), (Action<long>)delegate
			{
				OpenLoadingWindow();
			}));
			return Observable.SelectMany<Date, Date>(Observable.Do<Date>(Observable.Do<Date>(observable, (Action<Date>)delegate
			{
				CloseLoadingWindow();
			}), (Action<Date>)delegate
			{
				_eyeView.Reset();
				foreach (DatePhrase step in date.Steps)
				{
					step.Uncomplete();
				}
				_dateFlowController.StartDialogue(new Queue<DatePhrase>(date.Steps));
			}), (Func<Date, IObservable<Date>>)((Date _) => Observable.Select<Unit, Date>(Observable.Take<Unit>(_dateFlowController.OnEnd, 1), (Func<Unit, Date>)((Unit __) => date))));
		})), (Action<Date>)CompleteDate);
		_dateSoundController.Init();
	}

	private void OpenLoadingWindow()
	{
		if (_loadingWindow == null)
		{
			_loadingWindow = _windowsManager.Get<DateLoadingWindow>();
		}
		_loadingWindow.Open();
		_loadingWindow.PlayAnimations();
	}

	private void CloseLoadingWindow()
	{
		if (_loadingWindow != null)
		{
			_loadingWindow.Close();
			_loadingWindow.StopAnimations();
		}
	}

	private void CompleteDate(Date date)
	{
		date.SetState(EntityStatus.Complete);
		ReleaseDateData(date);
		_dateSoundController.SetActiveMusic(isActive: true);
	}

	private void ReleaseDateData(Date date)
	{
		_dateUnloadService.Unload(date);
	}

	public void Dispose()
	{
		_dateStartRequested.Dispose();
		_startStream?.Dispose();
	}
}
