using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Dates.Providers;

public class DateProvider : SimpleManager<Date>
{
	public Date Get(int id)
	{
		return collection.FirstOrDefault((Date x) => x.ID == id);
	}

	public IEnumerable<Date> GetByCharacter(ICharacter character)
	{
		return collection.Where((Date x) => x.Steps.FirstOrDefault((DatePhrase x) => x.CharacterID == character.ID) != null);
	}
}
