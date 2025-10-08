using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public sealed class HideIfAttribute : Attribute
{
	public HideIfAttribute(string condition, bool animate = true)
	{
	}

	public HideIfAttribute(string condition, object optionalValue, bool animate = true)
	{
	}
}
