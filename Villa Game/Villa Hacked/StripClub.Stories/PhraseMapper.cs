using System;
using StripClub.Model;

namespace StripClub.Stories;

[Serializable]
public class PhraseMapper
{
	public int story_id;

	public int step;

	public int[,] characters_visible;

	public int character_data;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public PhraseMapper(int story_id, int step, int[,] characters_visible, int character_data, UnlockType[] unlock_type, string[] unlock_value)
	{
		this.story_id = story_id;
		this.step = step;
		this.characters_visible = characters_visible;
		this.character_data = character_data;
		this.unlock_type = unlock_type;
		this.unlock_value = unlock_value;
	}
}
