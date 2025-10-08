using TMPro;
using UnityEngine;

public class BuildVersion : MonoBehaviour
{
	[SerializeField]
	private TMP_Text versionText;

	private void Awake()
	{
		string version = Application.version;
		versionText.text = version;
	}
}
