using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using Zenject;

namespace GreenT.HornyScapes.Cheats.UI;

public class CheatAddSkin : CheatButtonWithInputField
{
	private SkinManager skinManager;

	private CharacterSettingsManager characterManager;

	private Skin skin;

	[Inject]
	public void Init(SkinManager skinManager, CharacterSettingsManager characterManager)
	{
		this.skinManager = skinManager;
		this.characterManager = characterManager;
	}

	public override void Apply()
	{
		skinManager.Get(skin.ID).Own();
		inputField.text = string.Empty;
	}

	public override bool IsValid(string param)
	{
		if (!int.TryParse(param, out var skinID))
		{
			return false;
		}
		skin = skinManager.Collection.FirstOrDefault((Skin _x) => _x.ID == skinID);
		return skin != null;
	}
}
