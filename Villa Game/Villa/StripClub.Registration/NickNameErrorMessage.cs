using Zenject;

namespace StripClub.Registration;

public class NickNameErrorMessage : ErrorMessage
{
	[Inject]
	private void Init(NickNameChecker nickNameChecker)
	{
		_checker = nickNameChecker;
	}
}
