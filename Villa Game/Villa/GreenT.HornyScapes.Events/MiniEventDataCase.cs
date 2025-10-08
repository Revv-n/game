using System;
using GreenT.Data;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public class MiniEventDataCase : ISavableState
{
	[Serializable]
	private class MiniEventDataCaseMemento : Memento
	{
		public bool StartWindowShown;

		public MiniEventDataCaseMemento(MiniEventDataCase data)
			: base(data)
		{
			StartWindowShown = data.StartWindowShown;
		}
	}

	private readonly ISaver _saver;

	private readonly string _saveKey;

	public bool StartWindowShown { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MiniEventDataCase(ISaver saver, string saveKey)
	{
		_saver = saver;
		_saveKey = saveKey;
		_saver.Add(this);
	}

	public string UniqueKey()
	{
		return _saveKey;
	}

	public Memento SaveState()
	{
		return new MiniEventDataCaseMemento(this);
	}

	public void LoadState(Memento memento)
	{
		MiniEventDataCaseMemento miniEventDataCaseMemento = (MiniEventDataCaseMemento)memento;
		StartWindowShown = miniEventDataCaseMemento.StartWindowShown;
	}
}
