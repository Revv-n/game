using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class TaskActivityStructureInitializer<T> : StructureInitializerViaArray<TaskActivityMapper, MiniEventTask> where T : MiniEventTask
{
	public TaskActivityStructureInitializer(IManager<MiniEventTask> manager, IFactory<TaskActivityMapper, T> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, (IFactory<TaskActivityMapper, MiniEventTask>)(object)factory, others)
	{
	}
}
