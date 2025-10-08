using UnityEngine;

public class ExampleSteamDataSuite : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button("Initialize SteamDataSuite", GUILayout.Width(200f)))
		{
			SteamDataSuite.Initialize();
		}
	}
}
