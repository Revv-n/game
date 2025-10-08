using StripClub.Extensions;
using TMPro;
using UnityEngine;

namespace StripClub.Registration;

public class TMProInputFieldMatchingChecker : MatchingChecker
{
	[SerializeField]
	private TMP_InputField _sourceObject;

	[SerializeField]
	private TMP_InputField _comparsionObject;

	public override string ComparsionObject => _comparsionObject.text.StripUnicodeCharactersFromString();

	public void ValidateLocally()
	{
		Validate(_sourceObject.text.StripUnicodeCharactersFromString());
	}
}
