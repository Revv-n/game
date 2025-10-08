using System;
using Hedge.Tools;
using UniRx;
using UnityEngine;

namespace Hedge.UI;

public abstract class NumericsDataSpreader : DataSpreader
{
	[SerializeField]
	private bool shortNumber = true;

	private IDisposable numberFormatStream;

	protected abstract void SetNumber(string number);

	protected override void ParameterHandler<T>(ParameterType txtType, T obj)
	{
		if (!this || dataType != txtType)
		{
			return;
		}
		numberFormatStream?.Dispose();
		if (!shortNumber)
		{
			SetNumber(obj.ToString());
			return;
		}
		numberFormatStream = obj.ToObservableShortNumber().Subscribe(delegate(string formatedValue)
		{
			SetNumber(formatedValue);
		}).AddTo(this);
	}

	protected override void OnDestroy()
	{
		DataSpreader.OnUpdate = (Action<ParameterType, object>)Delegate.Remove(DataSpreader.OnUpdate, new Action<ParameterType, object>(ParameterHandler));
		numberFormatStream?.Dispose();
	}
}
