using System.Text.RegularExpressions;

namespace StripClub.Registration;

public class NickNameChecker : RegexChecker
{
	private const string localizationPrefix = "ui.error.checker.nickname.";

	protected override string ErrorLocalizationPrefix => "ui.error.checker.nickname.";

	protected override Regex RegularExpression { get; set; } = new Regex("\\w{3,20}");

}
