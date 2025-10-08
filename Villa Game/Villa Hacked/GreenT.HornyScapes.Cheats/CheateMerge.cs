using GreenT.HornyScapes.MergeCore;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public abstract class CheateMerge : MonoBehaviour
{
	[Inject]
	protected InputSettingCheats inputSetting;

	protected GameItemController itemController => Controller<GameItemController>.Instance;

	protected MergeController mergeController => Controller<MergeController>.Instance;

	protected ClickSpawnController clickSpawnController => Controller<ClickSpawnController>.Instance;

	protected SelectionController selectionController => Controller<SelectionController>.Instance;
}
