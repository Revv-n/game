using System;

namespace GreenT.HornyScapes;

public abstract class BaseLastChanceStrategy
{
	public abstract event Action<LastChance> Stopped;

	public abstract void Init(LastChance lastChance, Action<LastChance> onFinish);

	public abstract void Execute(LastChance lastChance);

	public abstract void Stop(LastChance lastChance);
}
