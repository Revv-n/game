using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public sealed class ShowIfAttribute : Attribute
{
	public ShowIfAttribute(string condition, bool animate = true)
	{
	}

	public ShowIfAttribute(string condition, object optionalValue, bool animate = true)
	{
	}
}
