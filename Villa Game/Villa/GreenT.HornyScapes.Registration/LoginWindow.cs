using System;
using Cysharp.Threading.Tasks;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Registration.UI;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.UI;
using StripClub.Model;
using StripClub.Registration;
using StripClub.Registration.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Registration;

public class LoginWindow : PopupWindow
{
	private const string authorizationErrorKey = "ui.error.authorization.";

	[SerializeField]
	private TMP_InputField _emailField;

	[SerializeField]
	private TMP_InputField _passwordField;

	[SerializeField]
	private ErrorMessage _errorMessage;

	[SerializeField]
	private CheckIcon _emailCheckIcon;

	[SerializeField]
	private ActiveButton _loginButton;

	[SerializeField]
	private Button _registerButton;

	[SerializeField]
	private Button _closeButton;

	private AuthorizationRequestProcessor _authorizationProcessor;

	private IWindow _registrationWindow;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private IFactory<string, long, IErrorMessage> _errorMessageFactory;

	private GameStarter _gameStarter;

	[Inject]
	public void Init(AuthorizationRequestProcessor loginController, IFactory<string, long, IErrorMessage> errorMessageFactory, GameStarter gameStarter)
	{
		_authorizationProcessor = loginController;
		_errorMessageFactory = errorMessageFactory;
		_gameStarter = gameStarter;
	}

	private void OnEnable()
	{
		_loginButton.Button.OnClickAsObservable().Subscribe(LoginRequest).AddTo(_disposables);
		_registrationWindow = windowsManager.Get<RegistrationWindow>();
		_registerButton.OnClickAsObservable().Subscribe(delegate
		{
			_registrationWindow.Open();
		}).AddTo(_disposables);
		_closeButton.OnClickAsObservable().Subscribe(delegate
		{
			Close();
		}).AddTo(_disposables);
		_authorizationProcessor.AddListener(OnAuthorizationResponse, OnAuthorizationException);
	}

	private void OnAuthorizationException(Exception exception)
	{
		if (exception is UnityWebRequestException ex)
		{
			SetExceptionStatus(ex.ResponseCode);
		}
		else
		{
			SetExceptionStatus(0L);
		}
	}

	private void LoginRequest(Unit _)
	{
		string text = _emailField.text;
		string text2 = _passwordField.text;
		_authorizationProcessor.Request(text, text2);
	}

	private void OnAuthorizationResponse(Response<UserDataMapper> response)
	{
		if (response.Status == 0)
		{
			Close();
		}
		else
		{
			SetExceptionStatus(response.Status);
		}
	}

	private void SetExceptionStatus(long exceptionCode)
	{
		IErrorMessage errorMessage = _errorMessageFactory.Create("ui.error.authorization.", exceptionCode);
		_errorMessage.Set(errorMessage.GetMessage());
	}

	protected override void OnDisable()
	{
		_authorizationProcessor.RemoveListener(OnAuthorizationResponse);
		_authorizationProcessor.RemoveExceptionListener(OnAuthorizationException);
		base.OnDisable();
		_disposables.Clear();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_disposables.Dispose();
	}
}
