using System;
using UnityEngine;

namespace StripClub.UI;

[RequireComponent(typeof(LocalizedTextMeshPro))]
public class LocalizedTMProTimer : MonoTimer
{
	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	protected override void OnEveryUpdate(TimeSpan timeLeft)
	{
		localizedText.SetArguments(timerFormat(timeLeft));
	}

	private void OnValidate()
	{
		if (localizedText == null)
		{
			localizedText = GetComponent<LocalizedTextMeshPro>();
		}
	}
}
