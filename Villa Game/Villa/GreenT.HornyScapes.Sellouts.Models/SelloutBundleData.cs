using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Sellouts.Models;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Sellout/Bundle")]
public class SelloutBundleData : ScriptableObject
{
	[field: SerializeField]
	public Sprite BackgroundSprite { get; private set; }

	[field: SerializeField]
	public Sprite IconSprite { get; private set; }

	[field: SerializeField]
	public Sprite CurrencySprite { get; private set; }

	[field: SerializeField]
	public Sprite LeftRewardSprite { get; private set; }

	[field: SerializeField]
	public bool IsLeftRewardForeground { get; private set; }

	[field: SerializeField]
	public Sprite RightRewardSprite { get; private set; }

	[field: SerializeField]
	public bool IsRightRewardForeground { get; private set; }

	[field: SerializeField]
	public TMP_SpriteAsset SpriteAsset { get; private set; }

	[field: SerializeField]
	public Sprite[] FrameRewardSprites { get; private set; }
}
