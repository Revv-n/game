using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Events;

public class BattlePassMapperProvider : SimpleManager<BattlePassMapper>
{
	public BattlePassMapper GetEventMapper(int id)
	{
		return collection.FirstOrDefault((BattlePassMapper _event) => _event.bp_id == id);
	}
}
