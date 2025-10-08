using UnityEngine;
using Zenject;

namespace StripClub.Registration;

public class EmailErrorMessage : ErrorMessage
{
	[SerializeField]
	private string _busyMessage;

	[Inject]
	private void Init(EmailAvailabilityChecker emailChecker)
	{
		_checker = emailChecker;
	}
}
