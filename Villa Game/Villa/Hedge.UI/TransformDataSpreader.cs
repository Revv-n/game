using System.Collections.Generic;
using UnityEngine;

namespace Hedge.UI;

[RequireComponent(typeof(RectTransform))]
public class TransformDataSpreader : DataSpreader
{
	private RectTransform mask;

	[SerializeField]
	private int minX;

	[SerializeField]
	private int maxX;

	protected static Dictionary<ParameterType, object> MaxParamDict = new Dictionary<ParameterType, object>();

	public static void ForceSetMaxParameter(ParameterType type, object obj)
	{
		MaxParamDict.Add(type, obj);
	}

	protected override void ParameterHandler<T>(ParameterType type, T parameter)
	{
		if (!this || dataType != type)
		{
			return;
		}
		if (!mask)
		{
			mask = GetComponent<RectTransform>();
		}
		if ((bool)mask)
		{
			if (parameter is int)
			{
				int value = (parameter as int?).Value;
				TransformResize(value);
			}
			else if (parameter is float)
			{
				float parameter2 = (parameter as float?).Value;
				TransformResize(parameter2);
			}
			else if (parameter is double)
			{
				double parameter3 = (parameter as double?).Value;
				TransformResize(parameter3);
			}
			else
			{
				Debug.LogWarning("Для данного типа данных не написан сценарий обработки.");
			}
		}
	}

	private void TransformResize(float parameter)
	{
		if (!MaxParamDict.TryGetValue(dataType, out var value))
		{
			value = 0f;
		}
		if (value is float)
		{
			if (parameter > (float)value)
			{
				MaxParamDict[dataType] = parameter;
			}
			int num = maxX - minX;
			float num2 = parameter / (float)value;
			float x = ((num2 > 1f) ? 1f : num2) * (float)num + (float)minX;
			mask.sizeDelta = new Vector2(x, mask.sizeDelta.y);
		}
	}

	private void TransformResize(int parameter)
	{
		TransformResize((float)parameter);
	}

	private void TransformResize(double parameter)
	{
		TransformResize((float)parameter);
	}
}
