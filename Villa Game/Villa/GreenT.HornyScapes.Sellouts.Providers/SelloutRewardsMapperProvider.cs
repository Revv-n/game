using System;
using System.Linq;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Sellouts.Providers;

public class SelloutRewardsMapperProvider : SimpleManager<SelloutRewardsMapper>
{
	public SelloutRewardsMapper Get(int id)
	{
		SelloutRewardsMapper selloutRewardsMapper = collection.FirstOrDefault((SelloutRewardsMapper x) => x.id == id);
		if (selloutRewardsMapper == null)
		{
			new NullReferenceException($"No sellout rewards with id: {id}").LogException();
			return null;
		}
		return selloutRewardsMapper;
	}
}
