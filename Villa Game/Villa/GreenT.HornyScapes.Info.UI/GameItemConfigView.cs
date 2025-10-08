using Merge;
using StripClub.UI;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Info.UI;

public abstract class GameItemConfigView : MonoView<GIConfig>
{
	public Image Icon;

	public LocalizedTextMeshPro Total;

	public LocalizedTextMeshPro Bonus;

	public LocalizedTextMeshPro NoBonusPlaceholder;
}
