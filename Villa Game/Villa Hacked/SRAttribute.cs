using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SRAttribute : PropertyAttribute
{
	public class TypeInfo
	{
		public Type Type;

		public string Path;
	}

	private static Dictionary<Type, Type[]> _typeCache = new Dictionary<Type, Type[]>();

	public TypeInfo[] Types { get; private set; }

	public SRAttribute()
	{
		Types = null;
	}

	public SRAttribute(Type baseType)
	{
		if (baseType == null)
		{
			Debug.LogError("[SRAttribute] Incorrect type.");
		}
		Types = GetTypeInfos(GetChildTypes(baseType));
	}

	public SRAttribute(params Type[] types)
	{
		if (types == null || types.Length == 0)
		{
			Debug.LogError("[SRAttribute] Incorrect types.");
		}
		Types = GetTypeInfos(types);
	}

	public void SetTypeByName(string typeName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			Debug.LogError("[SRAttribute] Incorrect type name.");
		}
		Type typeByName = GetTypeByName(typeName);
		if (typeByName == null)
		{
			Debug.LogError("[SRAttribute] Incorrect type.");
		}
		Types = GetTypeInfos(GetChildTypes(typeByName));
	}

	public TypeInfo TypeInfoByPath(string path)
	{
		if (Types == null)
		{
			return null;
		}
		return Array.Find(Types, (TypeInfo p) => p.Path == path);
	}

	public static TypeInfo[] GetTypeInfos(Type[] types)
	{
		if (types == null)
		{
			return null;
		}
		TypeInfo[] array = new TypeInfo[types.Length];
		for (int i = 0; i < types.Length; i++)
		{
			array[i] = new TypeInfo
			{
				Type = types[i],
				Path = types[i].FullName
			};
		}
		return array;
	}

	public static Type[] GetChildTypes(Type type)
	{
		if (_typeCache.TryGetValue(type, out var value))
		{
			return value;
		}
		value = ((!type.IsInterface) ? (from t in Assembly.GetAssembly(type).GetTypes()
			where t.IsSubclassOf(type)
			select t).ToArray() : (from p in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly s) => s.GetTypes())
			where p != type && type.IsAssignableFrom(p)
			select p).ToArray());
		if (value != null)
		{
			_typeCache[type] = value;
		}
		return value;
	}

	public static Type GetTypeByName(string typeName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			return null;
		}
		string[] array = typeName.Split(char.Parse(" "), StringSplitOptions.None);
		return Type.GetType(string.Concat(str2: array[0], str0: array[1], str1: ", "));
	}

	public virtual void OnCreate(object instance)
	{
	}

	public virtual void OnChange(object instance)
	{
	}
}
