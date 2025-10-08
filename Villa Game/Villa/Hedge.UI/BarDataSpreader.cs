using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hedge.UI;

public class BarDataSpreader : DataSpreader
{
	[SerializeField]
	protected ParameterType max;

	[SerializeField]
	protected Image mask;

	private float currentValue;

	private float maxValue;

	protected override void Start()
	{
		base.Start();
		if (DataSpreader.cacheObjDict.TryGetValue(max, out var value))
		{
			ParameterHandler(max, value);
		}
	}

	protected override void ParameterHandler<T>(ParameterType type, T obj)
	{
		if (type.Equals(dataType) || type.Equals(max))
		{
			float current = ParseFloatValue(obj);
			if (type == dataType)
			{
				SetProgress(current, maxValue);
			}
			if (type == max)
			{
				SetProgress(currentValue, current);
			}
		}
	}

	private static float ParseFloatValue<T>(T obj)
	{
		if (float.TryParse(obj.ToString(), out var result))
		{
			return result;
		}
		Debug.LogError("No scenario for this type of data :" + obj.GetType().ToString());
		throw new ArgumentException("No scenario for this type of data :" + obj.GetType().ToString());
	}

	private void SetProgress(float current, float max)
	{
		currentValue = current;
		maxValue = max;
		if (max != 0f)
		{
			mask.fillAmount = Mathf.Clamp(current / max, 0f, 1f);
		}
	}

	private void OnValidate()
	{
		if (dataType.Equals(max))
		{
			Debug.LogError("ParameterType of Current can't be equals Max");
			max = dataType + 1;
		}
	}
}
