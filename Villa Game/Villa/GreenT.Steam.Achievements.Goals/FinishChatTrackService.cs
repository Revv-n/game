using System;
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
		_messageManager = messageManager;
	}

	public override void Track()
	{
		_messageManager.OnUpdate.Where((MessengerUpdateArgs next) => next.Dialogue != null).SelectMany((MessengerUpdateArgs next) => next.Dialogue.OnUpdate.Where((Dialogue _dialogue) => _dialogue.IsComplete)).TakeWhile((Dialogue _) => !IsComplete())
			.Subscribe(delegate
			{
				UpdateStats();
			})
			.AddTo(streams);
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
		streams?.Dispose();
	}
}
