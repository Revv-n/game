using System;
using System.Linq;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Sellouts.Providers;

public class SelloutMapperProvider : SimpleManager<SelloutMapper>
{
	public SelloutMapper Get(int id)
	{
		SelloutMapper selloutMapper = collection.FirstOrDefault((SelloutMapper x) => x.id == id);
		if (selloutMapper == null)
		{
			new NullReferenceException($"No sellout with id: {id}").LogException();
			return null;
		}
		return selloutMapper;
	}
}
