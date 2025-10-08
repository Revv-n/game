using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public class GUIColorAttribute : Attribute
{
	public GUIColorAttribute(float r, float g, float b, float a = 1f)
	{
	}

	public GUIColorAttribute(string getColor)
	{
	}
}
