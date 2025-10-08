using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GreenT.HornyScapes.Animations;
using GreenT.Net;
using GreenT.Net.User;
using StripClub.Model;
using StripClub.Registration;
using StripClub.Registration.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Registration.UI;

public class RegistrationWindow : PopupWindow
{
	private const string registrationErrorKey = "ui.error.registration.";

	[SerializeField]
	private Button close;

	[SerializeField]
	private ActiveButton createButton;

	[SerializeField]
	private TMP_InputField emailField;

	[SerializeField]
	private TMP_InputField passwordField;

	[SerializeField]
	private AbstractChecker[] checkers;

	[SerializeField]
	private ErrorMessage errorMessage;

	[SerializeField]
	private CheckIcon emailCheckIcon;

	private RegistrationRequestProcessor registrationController;

	private NicknameWindow nicknameWindow;

	private CompositeDisposable disposables = new CompositeDisposable();

	private IFactory<string, long, IErrorMessage> errorMessageFactory;

	[Inject]
	public void Init(RegistrationRequestProcessor registrationController, IFactory<string, long, IErrorMessage> errorMessageFactory)
	{
		this.registrationController = registrationController;
		this.errorMessageFactory = errorMessageFactory;
	}

	private void Start()
	{
		nicknameWindow = windowsManager.Get<NicknameWindow>();
		createButton.Set(checkers);
	}

	private void OnEnable()
	{
		registrationController.AddListener(OnRegistrationResponseRecieved, OnException);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(createButton.Button), (Action<Unit>)RegistrationRequest), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(close), (Action<Unit>)delegate
		{
			Close();
		}), (ICollection<IDisposable>)disposables);
	}

	private void OnException(Exception obj)
	{
		if (obj is UnityWebRequestException ex)
		{
			SetStatus(ex.ResponseCode);
		}
		else
		{
			SetStatus(0L);
		}
	}

	private void RegistrationRequest(Unit _)
	{
		createButton.Button.interactable = false;
		string text = emailField.text;
		string text2 = passwordField.text;
		registrationController.Request(text, text2);
	}

	private void OnRegistrationResponseRecieved(Response response)
	{
		if (response.Status == 0)
		{
			Close();
		}
		else
		{
			SetStatus(response.Status);
		}
	}

	private void SetStatus(long status)
	{
		IErrorMessage errorMessage = errorMessageFactory.Create("ui.error.registration.", status);
		this.errorMessage.Set(errorMessage.GetMessage());
		emailCheckIcon.SetState(AbstractChecker.ValidationState.NotValid);
		createButton.Button.interactable = true;
	}

	protected override void OnDisable()
	{
		registrationController.RemoveListener(OnRegistrationResponseRecieved);
		base.OnDisable();
		disposables.Clear();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposables.Dispose();
	}
}
