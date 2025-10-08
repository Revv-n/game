using System;
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
		_locker = locker;
	}

	public override void Track()
	{
		IObservable<bool> source = (from x in _locker.IsOpen.TakeWhile((bool _) => !IsComplete())
			where x
			select x).Take(1);
		source.Where((bool x) => !Achievement.Achieved).Subscribe(SetProgress).AddTo(_disposables);
		source.Where((bool x) => Achievement.Achieved).Subscribe(SendErrorMsg).AddTo(_disposables);
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
		_disposables?.Dispose();
	}
}
