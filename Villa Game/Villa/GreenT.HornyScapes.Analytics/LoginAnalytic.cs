using GreenT.Net;
using GreenT.Net.User;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class LoginAnalytic : BaseAnalytic<User>
{
	private AuthorizationRequestProcessor registrationController;

	private PartnerSender partnerSender;

	public LoginAnalytic([Inject(Id = "Authorization")] AuthorizationRequestProcessor registrationController, PartnerSender partnerSender, IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
		this.registrationController = registrationController;
		this.partnerSender = partnerSender;
	}

	public override void Track()
	{
		registrationController.AddListener(OnLogin);
	}

	private void OnLogin(Response<UserDataMapper> answer)
	{
		if (answer.Status.Equals(0))
		{
			partnerSender.AddEvent(new LoginPartnerEvent());
		}
	}
}
