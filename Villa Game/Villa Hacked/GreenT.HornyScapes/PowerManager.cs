using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public class PowerManager : SimpleManager<PowerMapper>
{
	public PowerMapper GetPowerInfo(int id = 0)
	{
		if (id == 0)
		{
			return collection.First();
		}
		return collection.FirstOrDefault((PowerMapper _power) => _power.id == id);
	}
}
