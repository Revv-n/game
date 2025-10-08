using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class ProgressSliderTMPro : ProgressSlider
{
	[Header("Use {0}-for value; {1}-for max value; {2} - relative value")]
	[SerializeField]
	protected TextFormatDictionary tmproFields;

	public override void Init(float value, float max, float min = 0f)
	{
		base.Init(value, max, min);
		UpdateTextFields(value, max, min);
	}

	public void UpdateTextFields(float value, float max, float min = 0f)
	{
		float num = ((max != 0f) ? ((value - min) / (max - min) * 100f) : 0f);
		foreach (KeyValuePair<TextMeshProUGUI, string> tmproField in tmproFields)
		{
			tmproField.Key.text = string.Format(tmproField.Value, (int)value, max, num);
		}
	}
}
