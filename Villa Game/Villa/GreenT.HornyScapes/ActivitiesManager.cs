using System;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class ActivitiesManager : SimpleManager<ActivityMapper>
{
	public ActivityMapper GetActivityInfo(int id)
	{
		ActivityMapper activityMapper = collection.FirstOrDefault((ActivityMapper _activity) => _activity.id == id);
		if (activityMapper == null)
		{
			new Exception().SendException($"{GetType().Name}: activity with id {id} didn't load");
		}
		return activityMapper;
	}
}
