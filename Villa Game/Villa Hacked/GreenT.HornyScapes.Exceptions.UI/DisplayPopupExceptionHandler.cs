using System;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Exceptions.UI;

public class DisplayPopupExceptionHandler : MonoBehaviour, IExceptionHandler
{
	private string redirectUrl;

	private Exception ex;

	private GameStarter gameStarter;

	[field: SerializeField]
	public Window ExceptionPopupWindow { get; private set; }

	[Inject]
	public void Init(GameStarter gameStarter, [Inject(Id = "SupportUrl")] string redirectUrl)
	{
		this.redirectUrl = redirectUrl;
		this.gameStarter = gameStarter;
	}

	public void Handle(string reason)
	{
		Handle(new Exception(reason));
	}

	public void Handle(Exception innerEx, string reason)
	{
		Exception ex = new Exception(reason, innerEx);
		Handle(ex);
	}

	public void Handle(Exception ex)
	{
		if (this.ex == null)
		{
			this.ex = ex;
			this.ex.LogException();
			ExceptionPopupWindow.Open();
		}
	}

	public void CopyLog()
	{
		GUIUtility.systemCopyBuffer = ex.Message + "\n" + ex.StackTrace;
	}

	public void RedirectToSupport()
	{
		CopyLog();
		Application.OpenURL(redirectUrl);
	}

	public void RestartGame()
	{
		gameStarter.RestartApplication();
	}
}
