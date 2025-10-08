using UnityEngine;

namespace GreenT.HornyScapes.Characters.Skins;

[CreateAssetMenu(fileName = "CharacterView", menuName = "GreenT/HornyScapes/Character/Skin data")]
public class SkinData : ScriptableObject, ISkinData
{
	public const string assetBundleRootPath = "employee/skins/{0}/card_image";

	[SerializeField]
	private int skinID = -1;

	[SerializeField]
	private Sprite cardImage;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private Sprite progressBarIcon;

	[SerializeField]
	private Sprite squareIcon;

	[SerializeField]
	private Sprite splashArt;

	public int ID
	{
		get
		{
			return skinID;
		}
		set
		{
			skinID = value;
		}
	}

	public Sprite CardImage => cardImage;

	public Sprite Icon => icon;

	public Sprite ProgressBarIcon => progressBarIcon;

	public Sprite SquareIcon => squareIcon;

	public Sprite SplashArt => splashArt;
}
