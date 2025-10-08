using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[CreateAssetMenu(fileName = "MaskSettings", menuName = "GreenT/Tutorial/MaskSettings")]
public class MaskSettings : ScriptableObject
{
	[SerializeField]
	private Vector2 _holeSize = new Vector2(300f, 200f);

	[SerializeField]
	private Vector2 _holePosition = new Vector2(-150f, -100f);

	[SerializeField]
	private bool _raycastTarget;

	public Vector2 HoleSize => _holeSize;

	public Vector2 HolePosition => _holePosition;

	public bool RaycastTarget => _raycastTarget;
}
