using System.Text.RegularExpressions;

namespace StripClub.Registration;

public class PasswordChecker : RegexChecker
{
	private const string localizationPrefix = "ui.error.checker.password.";

	protected override Regex RegularExpression { get; set; } = new Regex("^\\S{6,16}$");


	protected override string ErrorLocalizationPrefix => "ui.error.checker.password.";
}
