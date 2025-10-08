using GreenT.Net;
using GreenT.Net.User;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class OnRegistrationSaveEvent : SaveEvent
{
	private RegistrationRequestProcessor registrationProcessor;

	[Inject]
	public void Init(RegistrationRequestProcessor registrationProcessor)
	{
		this.registrationProcessor = registrationProcessor;
	}

	public override void Track()
	{
		registrationProcessor.AddListener(OnRegistration);
	}

	private void OnDisable()
	{
		registrationProcessor.RemoveListener(OnRegistration);
	}

	private void OnRegistration(Response<UserDataMapper> response)
	{
		if (response.Status == 0)
		{
			Save();
		}
	}
}
