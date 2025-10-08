using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Characters.Data;

public class CharacterSettingsFactory : IFactory<ICharacter, CharacterSettings>, IFactory
{
	private readonly PromotePatterns promotePatterns;

	private readonly ISaver saver;

	public CharacterSettingsFactory(PromotePatterns promotePatterns, ISaver saver, DataCleanerManager _dataCleanerManager)
	{
		this.promotePatterns = promotePatterns;
		this.saver = saver;
	}

	public CharacterSettings Create(ICharacter character)
	{
		Dictionary<int, PromotePattern> promoteSettings = promotePatterns[character.PromotePatternsID][character.Rarity];
		CurrencyType currencyType = ((character.PromotePatternsID > 0) ? CurrencyType.Event : CurrencyType.Soft);
		Promote promote = new Promote(promoteSettings, currencyType);
		CharacterSettings characterSettings = new CharacterSettings(character, promote);
		saver.Add(characterSettings);
		characterSettings.Init();
		return characterSettings;
	}

	public IEnumerable<CharacterSettings> Create(IEnumerable<ICharacter> characters)
	{
		List<CharacterSettings> list = new List<CharacterSettings>();
		foreach (ICharacter character in characters)
		{
			CharacterSettings item = Create(character);
			list.Add(item);
		}
		return list;
	}
}
