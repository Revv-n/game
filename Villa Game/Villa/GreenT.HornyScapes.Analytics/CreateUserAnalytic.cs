using System;
using GreenT.Net;
using GreenT.Net.User;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class CreateUserAnalytic : BaseAnalytic<User>
{
	private RegistrationRequestProcessor registrationController;

	private PartnerSender partnerSender;

	private User user;

	private GameStarter gameStarter;

	private CompositeDisposable gameReadyStream = new CompositeDisposable();

	private IDisposable regStream;

	private IDisposable partnerStream;

	private Subject<Response<UserDataMapper>> onUpdate = new Subject<Response<UserDataMapper>>();

	public CreateUserAnalytic(GameStarter gameStarter, RegistrationRequestProcessor registrationController, User user, PartnerSender partnerSender, IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
		this.gameStarter = gameStarter;
		this.registrationController = registrationController;
		this.user = user;
		this.partnerSender = partnerSender;
	}

	public override void Track()
	{
		TrackRegisterEvent();
		TrackLogInEvent();
	}

	private void TrackRegisterEvent()
	{
		registrationController.AddListener(onUpdate.OnNext);
		regStream = user.OnUpdate.Select((User _) => (PlayerID: user.PlayerID, Type: user.Type)).StartWith((user.PlayerID, user.Type)).Pairwise()
			.Where(UserRegistrationCondition)
			.ZipLatest(onUpdate, (Pair<(string PlayerID, User.State Type)> _onLogin, Response<UserDataMapper> _onUpdate) => _onLogin)
			.Subscribe(delegate
			{
				partnerSender.AddEvent(new NewUserPartnerEvent());
			});
	}

	private void TrackLogInEvent()
	{
		gameReadyStream.Clear();
		(from _ in gameStarter.IsGameReadyToStart
			where _
			select user into _user
			where user.PlayerID != null
			select _user).Subscribe(delegate
		{
			partnerSender.AddEvent(new LoginPartnerEvent());
		}).AddTo(gameReadyStream);
	}

	private static bool UserRegistrationCondition(Pair<(string PlayerID, User.State Type)> _pair)
	{
		if (_pair.Current.PlayerID != _pair.Previous.PlayerID)
		{
			return _pair.Current.Type.Contains(User.State.Registered);
		}
		return false;
	}

	public override void Dispose()
	{
		base.Dispose();
		onUpdate.Dispose();
		gameReadyStream.Dispose();
		regStream?.Dispose();
		partnerStream?.Dispose();
	}
}
