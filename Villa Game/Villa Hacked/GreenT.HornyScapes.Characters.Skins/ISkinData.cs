using UnityEngine;

namespace GreenT.HornyScapes.Characters.Skins;

public interface ISkinData
{
	int ID { get; }

	Sprite CardImage { get; }

	Sprite Icon { get; }

	Sprite ProgressBarIcon { get; }

	Sprite SquareIcon { get; }

	Sprite SplashArt { get; }
}
