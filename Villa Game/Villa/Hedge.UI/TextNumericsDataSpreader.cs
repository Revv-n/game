using UnityEngine;
using UnityEngine.UI;

namespace Hedge.UI;

public class TextNumericsDataSpreader : NumericsDataSpreader
{
	[SerializeField]
	private Text text;

	protected override void SetNumber(string text)
	{
		this.text.text = text;
	}
}
