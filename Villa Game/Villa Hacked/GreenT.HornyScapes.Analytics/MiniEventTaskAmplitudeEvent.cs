namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventTaskAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "miniEvent_task";

	private const string TASK_ID = "task_id";

	private const string QUEST_MASSIVE_ID = "quest_massive_id";

	public MiniEventTaskAmplitudeEvent(int id, int questMassiveId)
		: base("miniEvent_task")
	{
		((AnalyticsEvent)this).AddEventParams("task_id", (object)id);
		((AnalyticsEvent)this).AddEventParams("quest_massive_id", (object)questMassiveId);
	}
}
