using GreenT.HornyScapes.Tasks;
using GreenT.Types;

namespace GreenT.HornyScapes.Events;

public class TaskDataCleaner : IDataCleaner
{
	private readonly TaskManagerCluster taskManagerCluster;

	public TaskDataCleaner(TaskManagerCluster taskManagerCluster)
	{
		this.taskManagerCluster = taskManagerCluster;
	}

	public void ClearData()
	{
		foreach (Task item in taskManagerCluster[ContentType.Event].Collection)
		{
			item.Initialize();
		}
	}
}
