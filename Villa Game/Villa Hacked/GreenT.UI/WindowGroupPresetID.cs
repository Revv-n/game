using System.Linq;
using UnityEngine;

namespace GreenT.UI;

[CreateAssetMenu(menuName = "GreenT/Window/WindowGroupPresetSO", fileName = "WindowGroupPresetSO")]
public class WindowGroupPresetID : WindowGroupID
{
	[SerializeField]
	private WindowGroupID[] windowGroupID;

	public override WindowID[] GetWindows()
	{
		return windowGroupID.SelectMany((WindowGroupID _group) => _group.GetWindows()).Concat(windows).ToArray();
	}
}
