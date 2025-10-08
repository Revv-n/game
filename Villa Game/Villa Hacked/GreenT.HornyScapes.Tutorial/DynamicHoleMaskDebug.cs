using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[ExecuteAlways]
public sealed class DynamicHoleMaskDebug : MonoBehaviour
{
	[SerializeField]
	private DynamicHoleMask _dynamicHoleMask;

	[SerializeField]
	private float _pixelsPerUnit = 1f;

	[Header("Debug Toggles")]
	[SerializeField]
	private bool _enableGizmos = true;

	[SerializeField]
	private bool _enableFill = true;

	[SerializeField]
	private bool _enableWireframe = true;

	[SerializeField]
	private bool _enableLabels = true;

	[Header("Colors")]
	[SerializeField]
	private Color _colorBlock = new Color(1f, 0f, 0f, 0.5f);

	[SerializeField]
	private Color _colorAllow = new Color(0f, 1f, 0f, 0.5f);

	[SerializeField]
	private Color _colorIntersectionFill = new Color(1f, 1f, 0f, 0.3f);

	[SerializeField]
	private Color _colorIntersectionOutline = new Color(1f, 1f, 0f, 0.8f);
}
