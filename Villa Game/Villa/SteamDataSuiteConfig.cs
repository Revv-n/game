using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SteamDataSuiteConfig", menuName = "SteamDataSuite/Config-file")]
public class SteamDataSuiteConfig : ScriptableObject
{
	[Tooltip("Easy way to disable it all together")]
	public bool Enabled = true;

	[FormerlySerializedAs("LicenseKey")]
	[Tooltip("The API key required to connect to the server. Looks like this: 2e96eedce46544d9806de9dc3401c8007")]
	public string TrackingKey = "";

	[Tooltip("Enable if you want to Log info when the connection with the server is successful/failed")]
	public bool DebugMode;
}
