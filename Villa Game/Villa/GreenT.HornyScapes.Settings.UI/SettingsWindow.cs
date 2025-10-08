using System;
using System.Collections.Generic;
using System.IO;
using Erolabs.Sdk.Unity;
using Games.Coresdk.Unity;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Erolabs;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Monetization.Android.Erolabs;
using GreenT.UI;
using Merge;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class SettingsWindow : PopupWindow
{
	[SerializeField]
	private Button closeBtn;

	[SerializeField]
	private List<StatableComponent> statableComponents;

	[SerializeField]
	private Button button;

	[SerializeField]
	private ManualSaveButton saveBtn;

	[SerializeField]
	private LocalizedTextMeshPro userMail;

	[SerializeField]
	private LocalizedTextMeshPro userID;

	[SerializeField]
	private TMP_InputField nicknameInputField;

	[SerializeField]
	private BuildVersion buildVersion;

	[SerializeField]
	private WindowID registrationWindowID;

	[SerializeField]
	private GameObject erolabsBindWarningMessage;

	private User userData;

	private IDisposable openLootboxStream;

	private IDisposable closeButtonStream;

	private IDisposable userUpdateStream;

	private IDisposable clickStream;

	private SavableVariable<bool> isRewardedForRegistration;

	private int lootboxRegistrationRewardID;

	private ILootboxOpener lootboxOpener;

	private LootboxCollection lootboxCollection;

	private IDataStorage dataStorage;

	private GameStarter gameStarter;

	private ErolabsMonetizationPopupOpener erolabsMonetizationPopupOpener;

	private ErolabsSDKAuthorization erolabsSDKAuthorization;

	private Hyena hyena;

	[Inject]
	public void Init(DiContainer container, User userData, SavableVariable<bool> registrationReward, IConstants<int> constants, ILootboxOpener lootboxOpener, LootboxCollection lootboxCollection, IDataStorage dataStorage, GameStarter gameStarter)
	{
		this.userData = userData;
		isRewardedForRegistration = registrationReward;
		lootboxRegistrationRewardID = constants["lootbox_registration"];
		this.lootboxOpener = lootboxOpener;
		this.lootboxCollection = lootboxCollection;
		this.dataStorage = dataStorage;
		this.gameStarter = gameStarter;
		if (PlatformHelper.IsErolabsMonetization())
		{
			erolabsSDKAuthorization = container.Resolve<ErolabsSDKAuthorization>();
			erolabsMonetizationPopupOpener = container.Resolve<ErolabsMonetizationPopupOpener>();
			hyena = container.Resolve<Hyena>();
		}
	}

	private void Start()
	{
		TrackRegistrationReward();
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsSteamMonetization() || PlatformHelper.IsHaremMonetization() || PlatformHelper.IsErolabsMonetization())
		{
			nicknameInputField.onEndEdit.AddListener(UpdateNickname);
		}
		else if (PlatformHelper.IsNutakuMonetization())
		{
			nicknameInputField.enabled = false;
		}
		Setup();
	}

	public override void Open()
	{
		base.Open();
		Setup();
	}

	private void Setup()
	{
		SetUserId();
		int num = 0;
		bool flag = userData.Type.Contains(User.State.Registered);
		if (flag)
		{
			num = ((!isRewardedForRegistration.Value) ? 1 : 2);
			SetUserMail();
		}
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(num);
		}
		nicknameInputField.interactable = flag;
		if (nicknameInputField.placeholder is TMP_Text tMP_Text)
		{
			tMP_Text.text = userData.Nickname;
		}
		clickStream?.Dispose();
		SubscribeRegisterBtn(num);
		if (PlatformHelper.IsErolabsMonetization())
		{
			erolabsBindWarningMessage.SetActive(userData.IsGuest);
		}
	}

	private void SetUserMail()
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			userMail.SetArguments(userData.Email);
		}
		else if (PlatformHelper.IsNutakuMonetization() || PlatformHelper.IsSteamMonetization())
		{
			userMail.SetArguments(dataStorage.HasKey("nickname") ? dataStorage.GetString("nickname") : "");
		}
		else if (PlatformHelper.IsErolabsMonetization())
		{
			userMail.SetArguments(userData.ErolabsNick);
		}
	}

	private void SubscribeRegisterBtn(int value)
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			switch (value)
			{
			case 0:
				clickStream = button.OnClickAsObservable().Subscribe(OpenRegistrationWindow);
				break;
			case 1:
				clickStream = button.OnClickAsObservable().TakeWhile((Unit _) => !isRewardedForRegistration.Value).Subscribe(ClaimReward);
				break;
			}
		}
		if (!PlatformHelper.IsErolabsMonetization())
		{
			return;
		}
		switch (value)
		{
		case 0:
			clickStream = button.OnClickAsObservable().Subscribe(delegate
			{
				erolabsMonetizationPopupOpener.ForceShowBinding();
				erolabsSDKAuthorization.BindAccount(erolabsMonetizationPopupOpener.ForceShowBindComplete);
			});
			break;
		case 1:
			clickStream = button.OnClickAsObservable().TakeWhile((Unit _) => !isRewardedForRegistration.Value).Subscribe(ClaimReward);
			break;
		case 2:
			clickStream = button.OnClickAsObservable().Subscribe(delegate
			{
				StartLogout();
			});
			break;
		}
	}

	private void SetUserId()
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsSteamMonetization() || PlatformHelper.IsHaremMonetization() || PlatformHelper.IsErolabsMonetization())
		{
			userID.SetArguments(userData.PlayerID);
		}
		else
		{
			PlatformHelper.IsNutakuMonetization();
		}
	}

	private void UpdateNickname(string nickname)
	{
		if (!nickname.Equals(userData.Nickname))
		{
			userData.Nickname = nickname;
		}
	}

	private void ClaimReward(Unit obj)
	{
		if (!isRewardedForRegistration.Value)
		{
			Lootbox lootbox = lootboxCollection.Get(lootboxRegistrationRewardID);
			lootboxOpener.Open(lootbox, CurrencyAmplitudeAnalytic.SourceType.SignUp);
		}
	}

	private void TrackRegistrationReward()
	{
		openLootboxStream?.Dispose();
		openLootboxStream = (from _lootbox in lootboxOpener.OnOpen.TakeWhile((Lootbox _) => !isRewardedForRegistration.Value)
			where lootboxRegistrationRewardID == _lootbox.ID
			select _lootbox).Subscribe(OnClaimRegistrationReward);
	}

	private void OnClaimRegistrationReward(Lootbox obj)
	{
		isRewardedForRegistration.Value = true;
		Setup();
	}

	private void OpenRegistrationWindow(Unit obj)
	{
		windowsManager.GetWindow(registrationWindowID).Open();
	}

	private void OnEnable()
	{
		closeButtonStream = closeBtn.OnClickAsObservable().Subscribe(delegate
		{
			Close();
		});
		userUpdateStream = userData.OnUpdate.Subscribe(delegate
		{
			Setup();
		});
		buildVersion.SetActive(active: true);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		closeButtonStream?.Dispose();
		userUpdateStream?.Dispose();
		buildVersion.SetActive(active: false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		openLootboxStream?.Dispose();
	}

	private void StartLogout()
	{
		ErolabsSDK.OpenLogout(OnLogout);
	}

	private void OnLogout(LogoutResult logoutResult)
	{
		if (logoutResult.Exception == null)
		{
			hyena.Logout();
			DeleteSave();
		}
	}

	private void DeleteSave()
	{
		DeletePlayerSaveFiles();
		DataManager.SimpleDeleteData();
		PlayerPrefs.DeleteAll();
		gameStarter.RestartApplication();
	}

	private void DeletePlayerSaveFiles()
	{
		string empty = string.Empty;
		try
		{
			string[] files = Directory.GetFiles(Application.persistentDataPath, empty ?? "");
			foreach (string filePath in files)
			{
				TryDeleteFile(filePath);
			}
		}
		catch (Exception)
		{
		}
	}

	private void TryDeleteFile(string filePath)
	{
		try
		{
			File.Delete(filePath);
		}
		catch (Exception)
		{
		}
	}
}
