using System.Linq;
using GreenT.Data;
using StripClub;

namespace GreenT.HornyScapes.Characters;

public class CharacterSettingsManager : CollectionManager<CharacterSettings>
{
	private ISaver saver;

	public CharacterSettingsManager(ISaver saver)
	{
		this.saver = saver;
	}

	public void Purge()
	{
		foreach (ISavableState item in collection.OfType<ISavableState>())
		{
			saver.Remove(item);
		}
		collection.Clear();
	}

	public CharacterSettings Get(int id)
	{
		return collection.Find((CharacterSettings unit) => unit.Public.ID == id);
	}

	internal void Remove(CharacterSettings settings)
	{
		collection.Remove(settings);
	}
}
