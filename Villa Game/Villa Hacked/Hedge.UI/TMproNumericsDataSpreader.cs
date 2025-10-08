using TMPro;
using UnityEngine;

namespace Hedge.UI;

public class TMproNumericsDataSpreader : NumericsDataSpreader
{
	[SerializeField]
	private TextMeshProUGUI tmpText;

	[SerializeField]
	private string format = "{0}";

	protected override void SetNumber(string text)
	{
		if (format != string.Empty)
		{
			tmpText.text = string.Format(format, text);
		}
		else
		{
			tmpText.text = text;
		}
	}
}
