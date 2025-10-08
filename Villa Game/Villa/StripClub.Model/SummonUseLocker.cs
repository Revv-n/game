using System;
using GreenT.Data;

namespace StripClub.Model;

[MementoHolder]
public class SummonUseLocker : Locker, ISavableState
{
	[Serializable]
	public class MergeMemento : Memento
	{
		public int count;

		public MergeMemento(SummonUseLocker locker)
			: base(locker)
		{
			count = locker._currentCount;
		}
	}

	private readonly int _targetCount;

	private readonly int _targetId;

	private readonly string _uniqueKey;

	private int _currentCount;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SummonUseLocker(int targetCount, int targetId)
	{
		_targetCount = targetCount;
		_targetId = targetId;
		_uniqueKey = $"{_targetId}_{_targetCount}_summon_use_locker";
	}

	public void Add(int value, int id)
	{
		if (id == _targetId && !base.IsOpen.Value)
		{
			_currentCount += value;
			TryOpen();
		}
	}

	public override void Initialize()
	{
	}

	public string UniqueKey()
	{
		return _uniqueKey;
	}

	public Memento SaveState()
	{
		return new MergeMemento(this);
	}

	public void LoadState(Memento memento)
	{
		MergeMemento mergeMemento = (MergeMemento)memento;
		_currentCount = mergeMemento.count;
		TryOpen();
	}

	private void TryOpen()
	{
		isOpen.Value = _currentCount >= _targetCount;
	}
}
