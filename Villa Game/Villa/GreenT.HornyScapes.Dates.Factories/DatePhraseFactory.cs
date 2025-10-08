using System;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Mappers;
using GreenT.HornyScapes.Dates.Models;
using Zenject;

namespace GreenT.HornyScapes.Dates.Factories;

public class DatePhraseFactory : IFactory<DatePhraseMapper, DatePhrase>, IFactory
{
	private const string LocalizationKey = "content.dates.{0}.{1}";

	private readonly CharacterManager _characterManager;

	public DatePhraseFactory(CharacterManager characterManager)
	{
		_characterManager = characterManager;
	}

	public DatePhrase Create(DatePhraseMapper mapper)
	{
		ICharacter character = null;
		try
		{
			if (mapper.character_data != 0)
			{
				character = _characterManager.Get(mapper.character_data);
			}
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
		string text = $"content.dates.{mapper.date_id}.{mapper.date_step}";
		return new DatePhrase(mapper.date_id, mapper.date_step, mapper.is_fade, mapper.characters_visible, mapper.character_data, mapper.backgrounds, mapper.background_sounds, mapper.sound_effects, mapper.is_changing_after_end, text, character?.NameKey);
	}
}
