using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public class HorizontalGroupAttribute : PropertyGroupAttribute
{
	public HorizontalGroupAttribute(string group, float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f)
		: base(group, order)
	{
	}

	public HorizontalGroupAttribute(float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f)
		: this("_DefaultHorizontalGroup", width, marginLeft, marginRight, order)
	{
	}
}
