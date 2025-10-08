using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatSpeedHack : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI timeScaleText;

	[SerializeField]
	private Slider slider;

	[SerializeField]
	private Button reset;

	[SerializeField]
	private float power = 10f;

	private int decimalNumbers = 2;

	private void Awake()
	{
		slider.onValueChanged.AddListener(delegate(float v)
		{
			SetTime(v * power);
		});
		reset.onClick.AddListener(delegate
		{
			SetTime(1f);
		});
	}

	private void SetTime(float timeScale)
	{
		timeScale = (float)Math.Round(timeScale, decimalNumbers);
		Time.timeScale = timeScale;
		timeScaleText.text = "TimeScale: " + timeScale;
	}
}
