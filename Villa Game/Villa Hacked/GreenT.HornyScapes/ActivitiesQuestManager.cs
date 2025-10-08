using System;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class ActivitiesQuestManager : SimpleManager<ActivityQuestMapper>
{
	public ActivityQuestMapper GetActivityInfo(int id)
	{
		ActivityQuestMapper activityQuestMapper = collection.FirstOrDefault((ActivityQuestMapper _event) => _event.tab_id == id);
		if (activityQuestMapper == null)
		{
			new Exception().SendException($"{GetType().Name}: activityQuest with id {id} didn't load");
		}
		return activityQuestMapper;
	}
}
