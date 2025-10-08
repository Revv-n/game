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
		_startStream = _dateStartRequested.Select(delegate(Date date)
		{
			IObservable<Date> observable = (from _ in _dateLoadService.LoadDate(date)
				select date).Publish().RefCount();
			Observable.Timer(TimeSpan.FromSeconds(_delay)).TakeUntil(observable).Do(delegate
			{
				OpenLoadingWindow();
			})
				.Subscribe();
			return observable.Do(delegate
			{
				CloseLoadingWindow();
			}).Do(delegate
			{
				_eyeView.Reset();
				foreach (DatePhrase step in date.Steps)
				{
					step.Uncomplete();
				}
				_dateFlowController.StartDialogue(new Queue<DatePhrase>(date.Steps));
			}).SelectMany((Date _) => from __ in _dateFlowController.OnEnd.Take(1)
				select date);
		}).Switch().Subscribe(CompleteDate);
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
