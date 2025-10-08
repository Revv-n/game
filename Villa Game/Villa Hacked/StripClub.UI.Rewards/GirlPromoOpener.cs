using GreenT.HornyScapes.Characters;
using StripClub.Model;
using StripClub.Model.Cards;
using Zenject;

namespace StripClub.UI.Rewards;

public class GirlPromoOpener
{
	private GirlPromoWindowWrapper girlPromoWindowWrapper;

	[Inject]
	private void InnerInit(GirlPromoWindowWrapper girlPromoWindowWrapper)
	{
		this.girlPromoWindowWrapper = girlPromoWindowWrapper;
	}

	public bool TryToOpenGirlPromo(ICharacter character)
	{
		if (!character.IsBundleDataReady)
		{
			return false;
		}
		girlPromoWindowWrapper.GirlPromoWindow?.OpenGirlPromo(character);
		return true;
	}

	public bool TryToOpenGirlPromo(ICard card)
	{
		if (!(card is ICharacter character))
		{
			return false;
		}
		return TryToOpenGirlPromo(character);
	}

	public bool TryToOpenGirlPromo(LinkedContent linkedContent)
	{
		if (!(linkedContent is CardLinkedContent cardLinkedContent))
		{
			return false;
		}
		return TryToOpenGirlPromo(cardLinkedContent.Card);
	}
}
