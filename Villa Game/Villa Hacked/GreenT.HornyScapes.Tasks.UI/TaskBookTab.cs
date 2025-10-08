using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskBookTab : MonoBehaviour
{
	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private SwitchStateWindowAdapter switcher;

	private void OnValidate()
	{
		if (!toggle)
		{
			toggle = GetComponent<Toggle>();
		}
	}

	public void Init()
	{
		toggle.onValueChanged.AddListener(switcher.SwitchState);
	}

	public void SetIsOnWithoutNotify(bool state)
	{
		toggle.SetIsOnWithoutNotify(state);
	}

	private void OnDestroy()
	{
		toggle.onValueChanged.RemoveAllListeners();
	}
}
