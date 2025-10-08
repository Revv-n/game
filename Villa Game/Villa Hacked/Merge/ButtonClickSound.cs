using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class ButtonClickSound : MonoBehaviour
{
	[SerializeField]
	private string soundName = "tap";

	private void Start()
	{
		GetComponent<Button>().AddClickCallback(delegate
		{
			Sounds.Play(soundName);
		});
	}
}
