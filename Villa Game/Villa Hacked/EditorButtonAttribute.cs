using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class EditorButtonAttribute : PropertyAttribute
{
	public string text = "";

	public bool showInfo = true;

	public EditorButtonAttribute()
	{
	}

	public EditorButtonAttribute(string text, bool showInfo = true)
	{
		this.text = text;
		this.showInfo = showInfo;
	}
}
