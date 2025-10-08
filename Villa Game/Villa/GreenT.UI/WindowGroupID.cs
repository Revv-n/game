using UnityEngine;

namespace GreenT.UI;

[CreateAssetMenu(menuName = "GreenT/Window/WindowGroupSO", fileName = "WindowGroupSO")]
public class WindowGroupID : ScriptableObject
{
	[SerializeField]
	protected WindowID[] windows;

	[SerializeField]
	private string description;

	public virtual WindowID[] GetWindows()
	{
		return windows;
	}
}
