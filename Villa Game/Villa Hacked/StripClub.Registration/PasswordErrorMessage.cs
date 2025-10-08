using Zenject;

namespace StripClub.Registration;

public class PasswordErrorMessage : ErrorMessage
{
	[Inject]
	private void Init(PasswordChecker passwordChecker)
	{
		_checker = passwordChecker;
	}
}
