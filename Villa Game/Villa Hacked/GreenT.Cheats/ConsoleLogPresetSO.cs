using UnityEngine;

namespace GreenT.Cheats;

[CreateAssetMenu(fileName = "ConsoleLogPreset", menuName = "GreenT/Cheats/Log/Preset")]
public class ConsoleLogPresetSO : ScriptableObject
{
	public LogType Target;
}
