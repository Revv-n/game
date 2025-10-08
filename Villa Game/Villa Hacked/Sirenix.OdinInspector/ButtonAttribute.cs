using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
[Conditional("UNITY_EDITOR")]
public class ButtonAttribute : Attribute
{
	public ButtonAttribute()
	{
	}

	public ButtonAttribute(ButtonSizes size)
	{
	}

	public ButtonAttribute(int buttonSize)
	{
	}

	public ButtonAttribute(string name)
	{
	}

	public ButtonAttribute(string name, ButtonSizes buttonSize)
	{
	}

	public ButtonAttribute(string name, int buttonSize)
	{
	}

	public ButtonAttribute(ButtonStyle parameterBtnStyle)
	{
	}

	public ButtonAttribute(int buttonSize, ButtonStyle parameterBtnStyle)
	{
	}

	public ButtonAttribute(ButtonSizes size, ButtonStyle parameterBtnStyle)
	{
	}

	public ButtonAttribute(string name, ButtonStyle parameterBtnStyle)
	{
	}

	public ButtonAttribute(string name, ButtonSizes buttonSize, ButtonStyle parameterBtnStyle)
	{
	}

	public ButtonAttribute(string name, int buttonSize, ButtonStyle parameterBtnStyle)
	{
	}
}
