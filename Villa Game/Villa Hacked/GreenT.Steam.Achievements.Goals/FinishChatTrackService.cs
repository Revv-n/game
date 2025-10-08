using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Steam.Achievements.Goals.Objectives;
using StripClub.Messenger;
using UniRx;

namespace GreenT.Steam.Achievements.Goals;

public class FinishChatTrackService : IntTrackService, IDisposable
{
	private IMessengerManager _messageManager;

	private CompositeDisposable streams = new CompositeDisposable();

	public FinishChatTrackService(AchievementService achievementService, IMessengerManager messageManager, AchievementDTO achievement, string statsKey, int targetValue)
		: base(achievementService, achievement, statsKey, targetValue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_messageManager = messageManager;
	}

	public override void Track()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(Observable.TakeWhile<Dialogue>(Observable.SelectMany<MessengerUpdateArgs, Dialogue>(Observable.Where<MessengerUpdateArgs>(_messageManager.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs next) => next.Dialogue != null)), (Func<MessengerUpdateArgs, IObservable<Dialogue>>)((MessengerUpdateArgs next) => Observable.Where<Dialogue>(next.Dialogue.OnUpdate, (Func<Dialogue, bool>)((Dialogue _dialogue) => _dialogue.IsComplete)))), (Func<Dialogue, bool>)((Dialogue _) => !IsComplete())), (Action<Dialogue>)delegate
		{
			UpdateStats();
		}), (ICollection<IDisposable>)streams);
		UpdateStats();
	}

	private void UpdateStats()
	{
		int num = (from _conversation in _messageManager.GetConversations()
			where _conversation.DialoguesFinished == _conversation.TotalDialoguesAmount
			select _conversation).Count((Conversation _conversation) => _conversation.IsComplete);
		AchievementService.SetStat(base.StatsKey, num);
		if (num >= base.TargetValue)
		{
			AchievementService.UnlockAchievement(Achievement);
		}
		AchievementService.UpdateStats();
	}

	public void Dispose()
	{
		CompositeDisposable obj = streams;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
