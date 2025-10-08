using System;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Testing;

public class TimeNode : MonoBehaviour
{
	[SerializeField]
	private float secondsInValue = 1f;

	[SerializeField]
	private Text label;

	[SerializeField]
	private Button plusBtn;

	[SerializeField]
	private Button minusBtn;

	[SerializeField]
	private string strFormat;

	public float SecondsInValue => secondsInValue;

	public float Value { get; private set; }

	public event Action<TimeNode> OnValueChange;

	private void Start()
	{
		plusBtn.AddClickCallback(IncreaseValue);
		minusBtn.AddClickCallback(DecreaseValue);
		Clear();
	}

	public void IncreaseValue()
	{
		Value += ((!Input.GetKey(KeyCode.LeftControl)) ? 1 : 10);
		label.text = $"{Value}{strFormat}";
	}

	public void DecreaseValue()
	{
		Value -= 1f;
		label.text = $"{Value}{strFormat}";
	}

	public void Clear()
	{
		Value = 0f;
		label.text = $"{Value}{strFormat}";
	}
}
