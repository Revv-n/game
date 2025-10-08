using System;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Characters;

public class CharacterManager : SimpleManager<ICharacter>
{
	public ICharacter Get(int characterID)
	{
		try
		{
			return Collection.First((ICharacter x) => x.ID.Equals(characterID));
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Exception on trying to find character by ID: " + characterID);
		}
	}
}
