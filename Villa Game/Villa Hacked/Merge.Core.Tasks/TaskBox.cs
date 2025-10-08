using System.Collections.Generic;
using System.Linq;

namespace Merge.Core.Tasks;

public class TaskBox
{
	public List<Objective> Objectives { get; private set; }

	public TaskConfig Config { get; private set; }

	public TaskData Data { get; private set; }

	public bool ReadyToComplete => !Objectives.Any((Objective x) => !x.IsCompleted);

	public TaskBox(TaskConfig config, TaskData data)
	{
		Data = data;
		Config = config;
		Objectives = new List<Objective>();
		foreach (ObjectiveInfo item in Config.ObjectivesInfo)
		{
			Objectives.Add(new Objective(item));
		}
	}
}
