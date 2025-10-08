using System.Collections.Generic;
using GreenT.UI;

namespace GreenT.HornyScapes.Collection;

public class ReturnButtonStrategy
{
	private Dictionary<ReturnToType, WindowOpener> strategies;

	private List<CollectionExitButton> listeners = new List<CollectionExitButton>();

	private ReturnToType currentState;

	public ReturnButtonStrategy(Dictionary<ReturnToType, WindowOpener> strategies)
	{
		this.strategies = strategies;
	}

	public void Add(CollectionExitButton listener)
	{
		if (!listeners.Contains(listener))
		{
			listener.Set(strategies[currentState]);
			listeners.Add(listener);
		}
	}

	public void Set(ReturnToType from)
	{
		if (currentState == from)
		{
			return;
		}
		currentState = from;
		foreach (CollectionExitButton listener in listeners)
		{
			listener.Set(strategies[currentState]);
		}
	}
}
