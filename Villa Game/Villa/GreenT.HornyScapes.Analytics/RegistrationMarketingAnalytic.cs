using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Tutorial;
using GreenT.Net;
using GreenT.Net.User;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class RegistrationMarketingAnalytic : BaseMarketingAnalytic
{
	private string postbackUrl;

	private const string ANALYTIC_EVENT = "registration_marketing";

	private const string TUTORIAL_STEP_ID_KEY = "tutorial_id_marketing";

	private int onCompleteStepID;

	private string ID;

	private CompositeDisposable sendStream = new CompositeDisposable();

	private CompositeDisposable trackTutorStream = new CompositeDisposable();

	private RegistrationRequestProcessor registrationProcessor;

	private TutorialSystem tutorialSystem;

	public RegistrationMarketingAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, RegistrationRequestProcessor registrationProcessor, TutorialSystem tutorialSystem, IConstants<int> constants, IUrlReader webglSiteReader, string url)
		: base(amplitude, webglSiteReader)
	{
		postbackUrl = url;
		this.registrationProcessor = registrationProcessor;
		this.tutorialSystem = tutorialSystem;
		onCompleteStepID = constants["tutorial_id_marketing"];
		Track();
	}

	public override void Track()
	{
		registrationProcessor.AddListener(Check);
		trackTutorStream.Clear();
		tutorialSystem.OnCompleteStep.Where((TutorialStep _step) => _step.StepID == onCompleteStepID).Subscribe(delegate
		{
			Check();
		}).AddTo(trackTutorStream);
	}

	protected void Check()
	{
		if (Validate())
		{
			SendEvent();
		}
	}

	protected void Check(Response<UserDataMapper> response)
	{
		Check();
	}

	protected bool Validate()
	{
		bool result = false;
		if (!PlayerPrefs.HasKey("registration_marketing"))
		{
			PlayerPrefs.SetString("registration_marketing", ReadIDFFromURL());
			result = true;
		}
		return result;
	}

	private void SendEvent()
	{
		ID = PlayerPrefs.GetString("registration_marketing");
		registrationProcessor.RemoveListener(Check);
		HttpRequestExecutor.GetRequest(string.Format(postbackUrl, ID)).Subscribe(delegate
		{
		}).AddTo(sendStream);
	}

	public override void Dispose()
	{
		registrationProcessor.RemoveListener(Check);
		base.Dispose();
		sendStream.Dispose();
		trackTutorStream.Dispose();
	}
}
