using System.Text.RegularExpressions;

namespace StripClub.Registration;

public abstract class RegexChecker : AbstractChecker
{
	protected abstract Regex RegularExpression { get; set; }

	protected override void Check(string input)
	{
		if (RegularExpression.IsMatch(input) && !string.IsNullOrEmpty(input))
		{
			SetState(ValidationState.IsValid);
		}
		else
		{
			SetState(ValidationState.NotValid, 1);
		}
	}
}
