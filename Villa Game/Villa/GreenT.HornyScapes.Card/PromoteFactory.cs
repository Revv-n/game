using System;
using GreenT.HornyScapes.Characters;
using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Card;

public class PromoteFactory : IFactory<ICard, IPromote>, IFactory
{
	private CharacterSettingsManager characterSettingsManager;

	private IFactory<ICharacter, CharacterSettings> settingsFactory;

	private PromotePatterns promotePatterns;

	public PromoteFactory(CharacterSettingsManager characterSettingsManager, IFactory<ICharacter, CharacterSettings> settingsFactory, PromotePatterns promotePatterns)
	{
		this.characterSettingsManager = characterSettingsManager;
		this.settingsFactory = settingsFactory;
		this.promotePatterns = promotePatterns;
	}

	public IPromote Create(ICard card)
	{
		if (card is ICharacter param)
		{
			CharacterSettings characterSettings = settingsFactory.Create(param);
			characterSettingsManager.Add(characterSettings);
			return characterSettings.Promote;
		}
		throw new NotImplementedException();
	}
}
