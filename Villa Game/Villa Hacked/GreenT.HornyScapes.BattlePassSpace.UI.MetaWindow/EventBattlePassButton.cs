using System;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassButton : MonoView
{
	[SerializeField]
	private WindowOpener _progressWindowOpener;

	public void Set(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		switch (eventCalendarModel.CalendarState.Value)
		{
		case EntityStatus.Blocked:
			ShowRewards();
			break;
		case EntityStatus.InProgress:
			ShowProgress(battlePass);
			break;
		case EntityStatus.Complete:
			ShowRewards();
			break;
		case EntityStatus.Rewarded:
			ShowRewards();
			break;
		default:
			throw new Exception().SendException($"{GetType().Name}: no behaviour for {eventCalendarModel.CalendarState.Value}");
		}
	}

	private void ShowRewards()
	{
		_progressWindowOpener.Click();
	}

	private void ShowProgress(BattlePass battlePass)
	{
		battlePass.Data.StartData.SetStartedBattlePassProgress();
		_progressWindowOpener.Click();
		battlePass.Data.StartData.SetFirstTimeStarted();
	}
}
