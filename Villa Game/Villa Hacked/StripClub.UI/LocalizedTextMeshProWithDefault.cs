using UnityEngine;

namespace StripClub.UI;

public class LocalizedTextMeshProWithDefault : LocalizedTextMeshPro
{
	[SerializeField]
	private string _defaultValue;

	protected override void SetText(string value)
	{
		if ((value.Equals(key) || string.IsNullOrEmpty(key)) && !string.IsNullOrEmpty(_defaultValue))
		{
			value = _defaultValue.Replace("\\n", "\n");
		}
		base.SetText(value);
	}
}
