using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassViewer : MonoView
{
	[SerializeField]
	private EventBattlePassProgressView _progressView;

	[SerializeField]
	private EventBattlePassPurchaseView _purchaseView;

	public void Set(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		_progressView.Set(eventCalendarModel, battlePass);
		_purchaseView.Set(eventCalendarModel, battlePass);
	}
}
