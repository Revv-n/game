using System.Collections.Generic;
using System.Linq;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskItemViewManager : ViewManager<MiniEventTask, MiniEventTaskItemView>
{
	public void MoveToFirst(MiniEventTaskItemView task)
	{
		views.Remove(task);
		views.Insert(0, task);
		task.transform.SetAsFirstSibling();
	}

	public void MoveToLast(MiniEventTaskItemView task)
	{
		int taskId = views.IndexOf(task);
		MiniEventTaskItemView miniEventTaskItemView = views.FirstOrDefault((MiniEventTaskItemView view) => !view.IsActive());
		if (miniEventTaskItemView == null)
		{
			Swap(taskId, views.Count - 1);
			return;
		}
		int objectId = views.IndexOf(miniEventTaskItemView);
		Swap(taskId, objectId);
	}

	public void MoveReadyTasksUp()
	{
		int num = views.IndexOf(views.FirstOrDefault((MiniEventTaskItemView view) => !view.Source.ReadyToComplete));
		if (num < 0)
		{
			return;
		}
		for (int i = num; i < base.VisibleViews.Count(); i++)
		{
			if (views[i].Source.ReadyToComplete && i >= num)
			{
				MoveToFirst(views[i]);
			}
		}
	}

	private void Swap(int taskId, int objectId)
	{
		List<MiniEventTaskItemView> list = views;
		List<MiniEventTaskItemView> list2 = views;
		MiniEventTaskItemView miniEventTaskItemView = views[objectId];
		MiniEventTaskItemView miniEventTaskItemView2 = views[taskId];
		MiniEventTaskItemView miniEventTaskItemView4 = (list[taskId] = miniEventTaskItemView);
		miniEventTaskItemView4 = (list2[objectId] = miniEventTaskItemView2);
		views[objectId].transform.SetSiblingIndex(objectId);
	}
}
