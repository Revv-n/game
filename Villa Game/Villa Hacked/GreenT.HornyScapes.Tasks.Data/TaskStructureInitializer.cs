using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Tasks.Data;

public class TaskStructureInitializer<T> : StructureInitializerViaArray<TaskMapper, Task> where T : Task
{
	public TaskStructureInitializer(IManager<Task> manager, IFactory<TaskMapper, T> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, (IFactory<TaskMapper, Task>)(object)factory, others)
	{
	}
}
