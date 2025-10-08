using System;
using TMPro;
using UnityEngine;

namespace StripClub.UI;

public class TMProTimer : MonoTimer
{
	[SerializeField]
	private TextMeshProUGUI timerText;

	protected override void OnEveryUpdate(TimeSpan timeLeft)
	{
		timerText.text = timerFormat(timeLeft);
	}

	private void OnValidate()
	{
		if (timerText == null)
		{
			timerText = GetComponent<TextMeshProUGUI>();
		}
	}
}
