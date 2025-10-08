using System;
using GreenT.Net;
using GreenT.Net.User;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class RegistrationAnalytic : BaseAnalytic<Response<UserDataMapper>>, IInitializable
{
	private const string ANALYTIC_EVENT = "registration";

	private readonly RegistrationRequestProcessor registrationController;

	private IDisposable partnerSendEventStream;

	private readonly IEvent signUpEvent;

	private readonly PartnerSender partnerSender;

	public RegistrationAnalytic(RegistrationRequestProcessor registrationController, IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
		this.registrationController = registrationController;
	}

	public void Initialize()
	{
		registrationController.AddListener(base.SendEventIfIsValid, 100);
	}

	public override void SendEventByPass(Response<UserDataMapper> tuple)
	{
		SendToAmplitude(tuple);
	}

	private void SendToAmplitude(Response<UserDataMapper> response)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("registration");
		amplitudeEvent.AddEventParams("registration", response.Data.EmailAdress);
		amplitude.AddEvent(amplitudeEvent);
	}

	private void SendToPartner(Response<UserDataMapper> response)
	{
		partnerSendEventStream?.Dispose();
		partnerSendEventStream = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
		{
			partnerSender.AddEvent(new SignUpPartnerEvent(response.Data.EmailAdress));
			signUpEvent.Send();
		});
	}

	public override void Dispose()
	{
		base.Dispose();
		registrationController.RemoveListener(base.SendEventIfIsValid);
		partnerSendEventStream?.Dispose();
	}
}
