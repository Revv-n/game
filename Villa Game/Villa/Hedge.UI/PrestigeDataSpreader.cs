using UnityEngine;
using UnityEngine.UI;

namespace Hedge.UI;

public class PrestigeDataSpreader : NumericsDataSpreader
{
	[SerializeField]
	private Text AdditionalText;

	protected override void SetNumber(string text)
	{
		if (AdditionalText != null)
		{
			AdditionalText.text = "+" + text + "% Profit";
		}
	}
}
