using System;
using UnityEngine;

public class SafeReflections : MonoBehaviour
{
	public static void SetFieldValue(object target, string name, object value)
	{
	}

	public static void SetPropertyValue(object target, string name, object value)
	{
	}

	public static object GetFieldValue(object target, string name)
	{
		return null;
	}

	public static T GetFieldValue<T>(object target, string name)
	{
		return default(T);
	}

	public static Type GetFieldType(object target, string name)
	{
		return null;
	}

	public static object GetValue(object target, string name)
	{
		return null;
	}

	public static void InvokeMethod(object target, string name, params object[] parametres)
	{
	}
}
