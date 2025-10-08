using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class ActivitiesTasksManager : SimpleManager<TaskActivityMapper>
{
	public List<TaskActivityMapper> GetTaskActivitiesInfo(int id)
	{
		List<TaskActivityMapper> list = collection.Where((TaskActivityMapper _task) => _task.massive_id == id).ToList();
		if (list == null || !list.Any())
		{
			new Exception().SendException($"{GetType().Name}: minievent Tasks with tab_id {id} didn't load");
		}
		return list;
	}
}
