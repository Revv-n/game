using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hedge.UI;

public abstract class DataSpreader : MonoBehaviour
{
	public ParameterType dataType;

	public static Action<ParameterType, object> OnUpdate;

	protected static Dictionary<ParameterType, object> cacheObjDict;

	static DataSpreader()
	{
		cacheObjDict = new Dictionary<ParameterType, object>();
		OnUpdate = (Action<ParameterType, object>)Delegate.Combine(OnUpdate, new Action<ParameterType, object>(CachedLastHandle));
	}

	private static void CachedLastHandle(ParameterType type, object obj)
	{
		cacheObjDict[type] = obj;
	}

	protected virtual void Start()
	{
		if (cacheObjDict.TryGetValue(dataType, out var value))
		{
			ParameterHandler(dataType, value);
		}
		OnUpdate = (Action<ParameterType, object>)Delegate.Combine(OnUpdate, new Action<ParameterType, object>(ParameterHandler));
	}

	protected virtual void OnDestroy()
	{
		OnUpdate = (Action<ParameterType, object>)Delegate.Remove(OnUpdate, new Action<ParameterType, object>(ParameterHandler));
	}

	protected abstract void ParameterHandler<T>(ParameterType type, T obj);
}
