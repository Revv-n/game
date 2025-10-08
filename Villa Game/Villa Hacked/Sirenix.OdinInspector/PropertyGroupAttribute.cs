using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public abstract class PropertyGroupAttribute : Attribute
{
	public PropertyGroupAttribute(string groupId, float order)
	{
	}

	public PropertyGroupAttribute(string groupId)
	{
	}
}
