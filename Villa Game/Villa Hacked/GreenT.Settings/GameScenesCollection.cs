using System.Collections.Generic;
using UnityEngine;

namespace GreenT.Settings;

[CreateAssetMenu(menuName = "GreenT/Settings/Scenes collection", order = 1)]
public class GameScenesCollection : ScriptableObject, IGameScenes
{
	[SerializeField]
	private List<SerializedScene> levelScenesLoadOrder;

	public IEnumerable<SerializedScene> LevelScenesLoadOrder => levelScenesLoadOrder;
}
