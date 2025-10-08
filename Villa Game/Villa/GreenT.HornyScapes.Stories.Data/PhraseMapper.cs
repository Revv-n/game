using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Stories.Data;

[Serializable]
[Mapper]
public class PhraseMapper : ILimitedContent
{
	public int story_id;

	public int step;

	public int[,] characters_visible;

	public int character_data;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public readonly ConfigContentType type;

	public ConfigContentType Type => type;

	public PhraseMapper(int story_id, int step, int[,] characters_visible, int character_data, UnlockType[] unlock_type, string[] unlock_value, ConfigContentType type)
	{
		this.story_id = story_id;
		this.step = step;
		this.characters_visible = characters_visible;
		this.character_data = character_data;
		this.unlock_type = unlock_type;
		this.unlock_value = unlock_value;
		this.type = type;
	}
}
