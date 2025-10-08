using System;
using GreenT.Data;

namespace GreenT.HornyScapes.Tasks;

[MementoHolder]
public class SavableObjectiveData : ObjectiveData, ISavableState
{
	[Serializable]
	public class ObjectiveCountableDataMemento : Memento
	{
		public int Progress;

		public ObjectiveCountableDataMemento(SavableObjectiveData savableObjective)
			: base(savableObjective)
		{
			Progress = savableObjective.Progress;
		}
	}

	private readonly int taskId;

	public int Progress;

	private readonly string uniqueKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SavableObjectiveData(int taskId, int required)
		: base(required)
	{
		this.taskId = taskId;
		uniqueKey = "objective_data_" + taskId;
	}

	public virtual string UniqueKey()
	{
		return uniqueKey;
	}

	public virtual Memento SaveState()
	{
		return new ObjectiveCountableDataMemento(this);
	}

	public virtual void LoadState(Memento memento)
	{
		ObjectiveCountableDataMemento objectiveCountableDataMemento = (ObjectiveCountableDataMemento)memento;
		Progress = objectiveCountableDataMemento.Progress;
	}
}
