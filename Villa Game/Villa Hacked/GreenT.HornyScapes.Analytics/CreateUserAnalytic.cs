using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		registrationController.AddListener((Action<Response<UserDataMapper>>)onUpdate.OnNext, 0);
		regStream = ObservableExtensions.Subscribe<Pair<(string, User.State)>>(Observable.ZipLatest<Pair<(string, User.State)>, Response<UserDataMapper>, Pair<(string, User.State)>>(Observable.Where<Pair<(string, User.State)>>(Observable.Pairwise<(string, User.State)>(Observable.StartWith<(string, User.State)>(Observable.Select<User, (string, User.State)>(user.OnUpdate, (Func<User, (string, User.State)>)((User _) => (PlayerID: user.PlayerID, Type: user.Type))), (user.PlayerID, user.Type))), (Func<Pair<(string, User.State)>, bool>)UserRegistrationCondition), (IObservable<Response<UserDataMapper>>)onUpdate, (Func<Pair<(string, User.State)>, Response<UserDataMapper>, Pair<(string, User.State)>>)((Pair<(string PlayerID, User.State Type)> _onLogin, Response<UserDataMapper> _onUpdate) => _onLogin)), (Action<Pair<(string, User.State)>>)delegate
		{
			partnerSender.AddEvent(new NewUserPartnerEvent());
		});
	}

	private void TrackLogInEvent()
	{
		gameReadyStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<User>(Observable.Where<User>(Observable.Select<bool, User>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameReadyToStart, (Func<bool, bool>)((bool _) => _)), (Func<bool, User>)((bool _) => user)), (Func<User, bool>)((User _user) => user.PlayerID != null)), (Action<User>)delegate
		{
			partnerSender.AddEvent(new LoginPartnerEvent());
		}), (ICollection<IDisposable>)gameReadyStream);
	}

	private static bool UserRegistrationCondition(Pair<(string PlayerID, User.State Type)> _pair)
	{
		if (_pair.Current.Item1 != _pair.Previous.Item1)
		{
			return _pair.Current.Item2.Contains(User.State.Registered);
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
