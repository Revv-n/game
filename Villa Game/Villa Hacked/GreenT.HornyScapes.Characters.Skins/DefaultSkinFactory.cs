using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins;

public class DefaultSkinFactory : IFactory<ICharacter, Skin>, IFactory
{
	public Skin Create(ICharacter character)
	{
		Skin skin = new Skin(new SkinMapper
		{
			girl_id = character.ID,
			id = 0,
			order_number = -4096,
			rarity = Rarity.Common,
			unlock_message = string.Empty
		}, character.PreloadLocker);
		ISkinData data = new CharacterAdapterSkinData(character);
		skin.Own();
		skin.Insert(data);
		return skin;
	}
}
