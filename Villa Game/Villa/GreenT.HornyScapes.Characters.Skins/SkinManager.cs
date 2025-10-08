using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinManager : SimpleManager<Skin>
{
	public Skin Get(int id)
	{
		try
		{
			return Collection.First((Skin x) => x.ID.Equals(id));
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Exception on trying to find skin by ID: " + id);
		}
	}

	public IEnumerable<Skin> GetSkinByCharacter(int characterID)
	{
		return Collection.Where((Skin _skin) => _skin.GirlID == characterID);
	}
}
