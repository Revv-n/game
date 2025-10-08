using UnityEngine;

namespace GreenT.HornyScapes.Events;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/MiniEvent/Bundle")]
public class MiniEventBundleData : ScriptableObject, IBundleData
{
	public Sprite TabBackground;

	public Sprite ContentBackground;

	public string Type;
}
