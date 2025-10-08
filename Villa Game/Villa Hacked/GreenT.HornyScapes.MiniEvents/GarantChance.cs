using System.Collections.Generic;
using System.Linq;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class GarantChance
{
	private Dictionary<int, int> _chances;

	public int ID { get; private set; }

	public GarantChance(int id)
	{
		ID = id;
		_chances = new Dictionary<int, int>();
	}

	public void AddConfiguration(int rollQuantity, int chance)
	{
		_chances.TryAdd(rollQuantity, chance);
	}

	public int GetMax()
	{
		return _chances.Max((KeyValuePair<int, int> x) => x.Key);
	}

	public int GetChance(int rollQuantity)
	{
		_chances.TryGetValue(rollQuantity, out var value);
		return value;
	}
}
