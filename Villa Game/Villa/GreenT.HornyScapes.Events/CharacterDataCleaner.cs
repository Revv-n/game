using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Characters;
using GreenT.Types;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Events;

public class CharacterDataCleaner : IDataCleaner
{
	private readonly CardsCollection cardsCollection;

	private readonly CharacterSettingsManager characterManager;

	private readonly ISaver saver;

	public CharacterDataCleaner(CardsCollection cardsCollection, CharacterSettingsManager characterManager, ISaver saver)
	{
		this.cardsCollection = cardsCollection;
		this.characterManager = characterManager;
		this.saver = saver;
	}

	public void ClearData()
	{
		ICard[] array = cardsCollection.Collection.Where((ICard _card) => _card.ContentType == ContentType.Event).ToArray();
		ICard[] array2 = array;
		foreach (ICard card in array2)
		{
			cardsCollection.Promote.Remove(card);
			if (card is ICharacter)
			{
				CharacterSettings characterSettings = characterManager.Get(card.ID);
				characterManager.Remove(characterSettings);
				if (characterSettings != null)
				{
					saver.Delete(characterSettings);
				}
			}
		}
		HotFix(array);
	}

	private void HotFix(ICard[] eventCards)
	{
		foreach (ICard card in eventCards)
		{
			saver.DeleteHashTablePoint(card.CharacterSaveKey());
		}
	}
}
