using System;
using System.Collections.Generic;
using GreenT.Steam.Achievements.Goals.Objectives;
using StripClub.Model;
using UniRx;

namespace GreenT.Steam.Achievements.Goals;

public class LockerTrackService : InstantTrackService, IDisposable
{
	private ILocker _locker;

	private CompositeDisposable _disposables = new CompositeDisposable();

	public LockerTrackService(AchievementService achievementService, AchievementDTO achievement, ILocker locker)
		: base(achievementService, achievement)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_locker = locker;
	}

	public override void Track()
	{
		IObservable<bool> observable = Observable.Take<bool>(Observable.Where<bool>(Observable.TakeWhile<bool>((IObservable<bool>)_locker.IsOpen, (Func<bool, bool>)((bool _) => !IsComplete())), (Func<bool, bool>)((bool x) => x)), 1);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(observable, (Func<bool, bool>)((bool x) => !Achievement.Achieved)), (Action<bool>)SetProgress), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(observable, (Func<bool, bool>)((bool x) => Achievement.Achieved)), (Action<bool>)SendErrorMsg), (ICollection<IDisposable>)_disposables);
	}

	private void SendErrorMsg(bool state)
	{
	}

	private void SetProgress(bool value)
	{
		AchievementService.UnlockAchievement(Achievement);
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
	}
}
