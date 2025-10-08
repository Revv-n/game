using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.Types;

namespace GreenT.HornyScapes.Stories;

public class StoryCluster : ContentCluster<StoryManager>
{
	public void Initialize()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.Initialize();
		}
	}

	public bool IsPhraseEnable(CharacterInfo characterID)
	{
		return IsPhraseEnable(characterID.ID);
	}

	public bool IsPhraseEnable(int characterID)
	{
		foreach (StoryManager value in base.Values)
		{
			foreach (Story item in value.Collection)
			{
				if (!item.IsComplete && item.Phrases.Any((StoryPhrase phrase) => !phrase.IsComplete && phrase.CharacterID == characterID))
				{
					return true;
				}
			}
		}
		return false;
	}
}
