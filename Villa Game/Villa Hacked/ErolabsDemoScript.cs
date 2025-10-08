using System;
using System.Collections;
using Erolabs.Sdk.Unity;
using Games.Coresdk.Unity;
using UnityEngine;

public class ErolabsDemoScript : MonoBehaviour
{
	private string game_id = "1";

	private string game_account = "game_001";

	private CoresdkDemoUI ui;

	private IEnumerator Start()
	{
		ui = GetComponent<CoresdkDemoUI>();
		ui.loginButton.onClick.AddListener(Login);
		ui.logoutButton.onClick.AddListener(Logout);
		ui.bindButton.onClick.AddListener(Bind);
		ui.purchaseButton.onClick.AddListener(Purchase);
		ui.postBindButton.onClick.AddListener(PostBind);
		yield return StartCoroutine(ErolabsSDK.Initialize(delegate(bool _)
		{
			Debug.Log("is initialized:" + _);
		}));
	}

	private void UpdateView(string text)
	{
		ui.viewText.text = text;
	}

	private void Login()
	{
		ErolabsSDK.OpenLogin(game_id, OnLoginCallback);
	}

	private void OnLoginCallback(ProfileResult result)
	{
		Exception exception = result.Exception;
		if (exception == null)
		{
			UpdateView("user_id:\n" + result.Data.user_info.user_id);
		}
		else
		{
			UpdateView("Exception:\n" + exception.ToString());
		}
	}

	private void Logout()
	{
		ErolabsSDK.OpenLogout(OnLogoutCallback);
	}

	private void Bind()
	{
		ErolabsSDK.OpenAccountBindGame(game_id, game_account, OnAccountLoginBindGameCallback);
	}

	private void Purchase()
	{
		ErolabsSDK.OpenPayment();
	}

	private void PostBind()
	{
		ErolabsSDK.PostAccountBindGame(game_id, game_account, OnAccountBindGameCallback);
	}

	private void OnLogoutCallback(LogoutResult result)
	{
		Exception exception = result.Exception;
		if (exception == null)
		{
			UpdateView("logout");
		}
		else
		{
			UpdateView("Exception:\n" + exception.ToString());
		}
	}

	private void OnAccountLoginBindGameCallback(BindProfileResult result)
	{
		Exception exception = result.Exception;
		if (exception == null)
		{
			UpdateView($"bind result:{result.IsSuccess}\nuser info:\naccount:{result.Data.user_info.account}");
		}
		else
		{
			UpdateView("Exception:\n" + exception.ToString());
		}
	}

	private void OnAccountBindGameCallback(AccountBindGameResult result)
	{
		UpdateView($"bind result:{result.Result}\nreason:{result.Reason}");
	}
}
