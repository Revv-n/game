using System.Net.Mail;

namespace StripClub.Registration;

public class EmailFormatChecker : AbstractChecker
{
	private const string localizationPrefix = "ui.error.checker.email.";

	protected MailAddress Adress;

	protected override string ErrorLocalizationPrefix => "ui.error.checker.email.";

	protected override void Check(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			SetState(ValidationState.Undefined);
			return;
		}
		try
		{
			Adress = new MailAddress(input);
			SetState(ValidationState.IsValid);
		}
		catch
		{
			SetState(ValidationState.NotValid, 1);
		}
	}
}
