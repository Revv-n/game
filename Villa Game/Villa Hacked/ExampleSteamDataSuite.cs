using UnityEngine;

public class ExampleSteamDataSuite : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button("Initialize SteamDataSuite", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(200f) }))
		{
			SteamDataSuite.Initialize();
		}
	}
}
