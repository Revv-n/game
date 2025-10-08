using System;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Lootboxes;

public class LootboxCollection : SimpleManager<Lootbox>
{
	public Lootbox Get(int id)
	{
		Lootbox lootbox = collection.FirstOrDefault((Lootbox _box) => _box.ID == id);
		if (lootbox != null)
		{
			return lootbox;
		}
		throw new ArgumentNullException("lootbox", $"{GetType().Name}: There is no lootbox with ID : {id} in collection").LogException();
	}
}
