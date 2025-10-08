using System;

namespace GreenT.HornyScapes.Dates.Mappers;

[Serializable]
public class DatePhraseMapper
{
	public int date_id;

	public int date_step;

	public bool is_fade;

	public int[,] characters_visible;

	public int character_data;

	public string[] backgrounds;

	public string[] background_sounds;

	public string[] sound_effects;

	public bool is_changing_after_end;
}
