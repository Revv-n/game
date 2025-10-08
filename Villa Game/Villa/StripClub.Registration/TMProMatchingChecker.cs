using StripClub.Extensions;
using TMPro;
using UnityEngine;

namespace StripClub.Registration;

public class TMProMatchingChecker : MatchingChecker
{
	[SerializeField]
	private TMP_Text comparsionObject;

	public override string ComparsionObject => comparsionObject.text.StripUnicodeCharactersFromString();
}
