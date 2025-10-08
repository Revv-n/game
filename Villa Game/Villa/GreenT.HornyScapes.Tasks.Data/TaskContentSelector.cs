using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.Types;

namespace GreenT.HornyScapes.Tasks.Data;

public class TaskContentSelector : IContentSelector, ISelector<ContentType>
{
	private readonly TaskManagerCluster managerCluster;

	private readonly TaskController taskController;

	private readonly MergeTaskViewManagerView mergeTaskViewManagerView;

	public TaskContentSelector(TaskManagerCluster managerCluster, TaskController taskController, MergeTaskViewManagerView mergeTaskViewManagerView)
	{
		this.managerCluster = managerCluster;
		this.taskController = taskController;
		this.mergeTaskViewManagerView = mergeTaskViewManagerView;
	}

	public void Select(ContentType type)
	{
		TasksManager tasksManager = managerCluster[type];
		if (Controller<TasksController>.Instance != null)
		{
			Controller<TasksController>.Instance.Set(tasksManager);
		}
		taskController.Set(tasksManager);
		mergeTaskViewManagerView.Set(tasksManager);
	}
}
