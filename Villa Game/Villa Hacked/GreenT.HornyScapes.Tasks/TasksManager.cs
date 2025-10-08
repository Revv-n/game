using System.Collections.Generic;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Tasks;

public class TasksManager : SimpleManager<Task>
{
	public IEnumerable<Task> Tasks => collection.AsEnumerable();

	public IEnumerable<IObjective> Objectives => collection.SelectMany((Task x) => x.Goal.Objectives);

	public IEnumerable<IObjective> ActiveObjectives => collection.Where((Task _task) => _task.IsActive).SelectMany((Task x) => x.Goal.Objectives);

	public Task GetItem(int id)
	{
		return collection.FirstOrDefault((Task _item) => _item.ID == id);
	}

	public int GetMaxId()
	{
		return collection.Max((Task _story) => _story.ID);
	}

	public Task GetLast()
	{
		int lastID = GetMaxId();
		return collection.Where((Task _story) => _story.ID == lastID).First();
	}

	public void Initialize()
	{
		foreach (Task item in Collection)
		{
			item.Initialize();
		}
	}
}
