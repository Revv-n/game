using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Tasks;
using GreenT.Model.Collections;
using GreenT.Types;

namespace GreenT.HornyScapes;

public class MiniEventTaskManager : SimpleManager<MiniEventTask>
{
	public IEnumerable<MiniEventTask> Tasks => collection.AsEnumerable();

	public IEnumerable<IObjective> Objectives => collection.SelectMany((MiniEventTask x) => x.Goal.Objectives);

	public IEnumerable<IObjective> ActiveObjectives => collection.Where((MiniEventTask _task) => _task.IsActive).SelectMany((MiniEventTask x) => x.Goal.Objectives);

	public void Initialize()
	{
		foreach (MiniEventTask item in Collection)
		{
			item.Initialize();
		}
	}

	public Task GetItem(CompositeIdentificator identificator)
	{
		return collection.FirstOrDefault((MiniEventTask _item) => _item.Identificator == identificator);
	}
}
