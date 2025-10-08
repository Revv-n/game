using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public sealed class EnableIfAttribute : Attribute
{
	public EnableIfAttribute(string condition)
	{
	}

	public EnableIfAttribute(string condition, object optionalValue)
	{
	}
}
