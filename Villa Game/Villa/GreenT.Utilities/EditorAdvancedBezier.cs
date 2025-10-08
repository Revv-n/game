using UnityEngine;

namespace GreenT.Utilities;

[ExecuteInEditMode]
public class EditorAdvancedBezier : AdvancedBezier
{
	[Header("используется только для настройки в редакторе")]
	public bool nothig;

	protected override void OnValidate()
	{
		base.OnValidate();
		Debug.Log("valid");
	}

	protected void Update()
	{
		OnValidate();
	}
}
