using System;
using UnityEngine;

namespace GreenT.Cheats;

[CreateAssetMenu(fileName = "ConsoleLogSettings", menuName = "GreenT/Cheats/Settings/LogSettings")]
public class ConsoleLogSettingsSO : ScriptableObject
{
	[Serializable]
	public class LogColorDicitionary : SerializableDictionary<LogType, Color>
	{
	}

	[SerializeField]
	private ConsoleLogPresetSO UsingPreset;

	public LogType Target;

	[field: SerializeField]
	public LogColorDicitionary LogColors { get; private set; }
}
