using System;
using GreenT.HornyScapes.Animations;
using GreenT.Net;
using GreenT.Net.User;
using StripClub.Model;
using StripClub.Registration;
using StripClub.Registration.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Registration.UI;

public class NicknameWindow : PopupWindow
{
	private const string nicknameErrorKey = "ui.error.nickname_setup.";

	[SerializeField]
	private ActiveButton playButton;

	[SerializeField]
	private TMP_Text nicknameField;

	[SerializeField]
	private AbstractChecker nicknameChecker;

	[SerializeField]
	private ErrorMessage errorMessage;

	[SerializeField]
	private CheckIcon nicknameChecIcon;

	private SetDataProcessor setDataProcessor;

	private User userData;

	private IFactory<string, long, IErrorMessage> errorMessageFactory;

	[Inject]
	public void Init(User userData, SetDataProcessor setDataProcessor, IFactory<string, long, IErrorMessage> errorMessageFactory)
	{
		this.setDataProcessor = setDataProcessor;
		this.userData = userData;
		this.errorMessageFactory = errorMessageFactory;
	}

	private void Start()
	{
		playButton.Set(nicknameChecker);
		setDataProcessor.AddListener(OnSetResponse);
	}

	private void OnEnable()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.TakeUntilDisable<Unit>(UnityUIComponentExtensions.OnClickAsObservable(playButton.Button), (Component)this), (Action<Unit>)SetNickName), (Component)this);
	}

	public void SetNickName(Unit _)
	{
		userData.Nickname = nicknameField.text;
		Close();
	}

	private void OnSetResponse(Response<UserDataMapper> response)
	{
		if (response.Status == 0)
		{
			Close();
			return;
		}
		IErrorMessage errorMessage = errorMessageFactory.Create("ui.error.nickname_setup.", (long)response.Status);
		this.errorMessage.Set(errorMessage.GetMessage());
		nicknameChecIcon.SetState(AbstractChecker.ValidationState.NotValid);
	}

	protected override void OnDestroy()
	{
		setDataProcessor.RemoveListener(OnSetResponse);
		base.OnDestroy();
	}
}
