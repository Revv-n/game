using StripClub.Extensions;
using UnityEngine;

namespace StripClub.Registration;

public abstract class MatchingChecker : AbstractChecker
{
	[SerializeField]
	private string localizationPrefix = "ui.error.checker.matching.";

	public abstract string ComparsionObject { get; }

	protected override string ErrorLocalizationPrefix => localizationPrefix;

	protected override void Check(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			SetState(ValidationState.Undefined);
			return;
		}
		input = input.StripUnicodeCharactersFromString();
		if (input.Equals(ComparsionObject))
		{
			SetState(ValidationState.IsValid);
		}
		else
		{
			SetState(ValidationState.NotValid, 1);
		}
	}
}
