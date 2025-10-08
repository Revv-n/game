using System.Collections.Generic;
using GreenT.Types;

namespace GreenT.HornyScapes.Tasks;

public class TaskManagerCluster : ContentCluster<TasksManager>
{
	public void Initialize()
	{
		foreach (TasksManager value in base.Values)
		{
			value.Initialize();
		}
	}

	public bool TryGetItem(int id, out Task task)
	{
		task = null;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Task item = enumerator.Current.Value.GetItem(id);
				if (item != null)
				{
					task = item;
					break;
				}
			}
		}
		return task != null;
	}
}
