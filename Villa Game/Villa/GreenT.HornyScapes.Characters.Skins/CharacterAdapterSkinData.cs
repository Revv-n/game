using UnityEngine;

namespace GreenT.HornyScapes.Characters.Skins;

public class CharacterAdapterSkinData : ISkinData
{
	public int ID => Source.ID;

	public Sprite CardImage => Source.GetBundleData().DefaultAvatar;

	public Sprite Icon => Source.GetBundleData().BankImages.Small;

	public Sprite ProgressBarIcon => Source.GetBundleData().ProgressBarIcon;

	public Sprite SquareIcon => Source.GetBundleData().SquareIcon;

	public Sprite SplashArt => Source.GetBundleData().SplashArt;

	public ICharacter Source { get; }

	public CharacterAdapterSkinData(ICharacter character)
	{
		Source = character;
	}
}
