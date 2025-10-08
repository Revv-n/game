using System;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Registration;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.UI;
using Merge;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class LoadingWindow : Window
{
	[SerializeField]
	private Button playButton;

	[SerializeField]
	private Button loginButton;

	private GameStarter gameStarter;

	private User userData;

	private ILoadingScreen loadingScreen;

	private SwitchingPhrases switchingPhrases;

	private SetDataProcessor setDataProcessor;

	private IDataStorage dataStorage;

	private IEvent playButtonEvent;

	private AuthorizationRequestProcessor authorizationRequestProcessor;

	private RestoreSessionProcessor restoreSessionProcessor;

	private UnityAction openLoginWindow;

	[Inject]
	public void Init(DiContainer container, ILoadingScreen loadingScreen, SwitchingPhrases switchingPhrases, User userData, GameStarter gameStarter, IDataStorage dataStorage, SetDataProcessor setDataProcessor)
	{
		this.loadingScreen = loadingScreen;
		this.switchingPhrases = switchingPhrases;
		this.userData = userData;
		this.gameStarter = gameStarter;
		this.dataStorage = dataStorage;
		this.setDataProcessor = setDataProcessor;
		if (PlatformHelper.IsEpochaMonetization())
		{
			playButtonEvent = container.Resolve<IEvent>();
		}
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			authorizationRequestProcessor = container.Resolve<AuthorizationRequestProcessor>();
			restoreSessionProcessor = container.Resolve<RestoreSessionProcessor>();
		}
	}

	private void Start()
	{
		LoginWindow loginWindow = windowsManager.Get<LoginWindow>();
		openLoginWindow = delegate
		{
			loginWindow.Open();
		};
		ShowControlButtons(show: false);
		loadingScreen.SetProgress(0.9f, 4f);
		gameStarter.LoadingProgress.OnProgressUpdate.SkipWhile((float _progress) => _progress < 0.9f).TakeWhile((float _progress) => _progress < 1f).Subscribe(delegate(float _progress)
		{
			loadingScreen.SetProgress(_progress, 5f);
		})
			.AddTo(this);
		gameStarter.LoadingProgress.OnProgressUpdate.StartWith(gameStarter.LoadingProgress.Progress).First((float _) => gameStarter.LoadingProgress.IsComplete()).Subscribe(delegate(float _progress)
		{
			loadingScreen.SetProgress(_progress, 1f);
		})
			.AddTo(this);
		if (!gameStarter.LoadingProgress.IsComplete())
		{
			switchingPhrases.StartAnimation();
		}
		gameStarter.IsGameReadyToStart.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			OnLoadingComplete();
		})
			.AddTo(this);
		gameStarter.IsGameReadyToStart.ToReactiveCommand(initialValue: false).BindTo(playButton).AddTo(this);
		gameStarter.IsGameActive.Subscribe(delegate(bool x)
		{
			loadingScreen.SetLoadingScreenActive(!x);
		}).AddTo(this);
		gameStarter.IsConfigsLoaded.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			loadingScreen.SetOnConfigsLoaded();
		});
	}

	private void OnLoadingComplete()
	{
		switchingPhrases.StopAnimation();
		ActivateButtons();
		setDataProcessor.AddListener(OnSetData);
		if (PlatformHelper.IsEpochaMonetization())
		{
			playButtonEvent?.Send();
		}
		if ((PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization()) && userData.Equals(User.Unknown))
		{
			authorizationRequestProcessor?.AddListener(OnAuthorizate);
		}
	}

	private void OnSetData(Response<UserDataMapper> response)
	{
		if (response.Status == 0)
		{
			setDataProcessor.RemoveListener(OnSetData);
			StartGame();
		}
	}

	private void OnAuthorizate(Response<UserDataMapper> obj)
	{
		if (obj.Status == 0)
		{
			authorizationRequestProcessor?.RemoveListener(OnAuthorizate);
			loginButton.gameObject.SetActive(value: false);
		}
	}

	private void ActivateButtons()
	{
		playButton.onClick.RemoveListener(StartGame);
		playButton.onClick.AddListener(StartGame);
		loginButton.onClick.RemoveListener(openLoginWindow);
		loginButton.onClick.AddListener(openLoginWindow);
		ShowControlButtons(show: true);
	}

	private void ShowControlButtons(bool show)
	{
		playButton.gameObject.SetActive(show);
		SetViewLoginButton(show);
	}

	private void SetViewLoginButton(bool show)
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			loginButton.gameObject.SetActive(show && !userData.Type.Contains(User.State.Registered));
			loginButton.gameObject.SetActive(show && !userData.Type.Contains(User.State.Registered));
			return;
		}
		if (PlatformHelper.IsNutakuMonetization() || PlatformHelper.IsSteamMonetization() || PlatformHelper.IsErolabsMonetization())
		{
			loginButton.SetActive(active: false);
			return;
		}
		throw new NotImplementedException();
	}

	private bool IsPopupMode()
	{
		bool.TryParse(dataStorage.GetString("popup_mode"), out var result);
		return result;
	}

	private void StartGame()
	{
		gameStarter.Play();
	}

	protected override void OnDestroy()
	{
		setDataProcessor.RemoveListener(OnSetData);
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			authorizationRequestProcessor?.RemoveListener(OnAuthorizate);
		}
		base.OnDestroy();
	}
}
