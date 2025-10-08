using System;

namespace Merge;

public interface IControlClocks
{
	int ClockControlPriority { get; }

	bool IsTimerVisible { get; }

	event Action<bool> OnTimerActiveChange;

	event Action<IControlClocks> OnTimerComplete;

	event Action<TimerStatus> OnTimerTick;
}
