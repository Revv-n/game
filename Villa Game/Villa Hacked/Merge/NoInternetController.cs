using System;
using UnityEngine;

namespace Merge;

public class NoInternetController : Controller<NoInternetController>
{
	[SerializeField]
	private NoInternetWindow window;

	public static bool HasInternet => Controller<NoInternetController>.Instance.CheckInternet();

	public static bool NoInternet => !Controller<NoInternetController>.Instance.CheckInternet();

	public event Action<bool> OnInternetChange;

	private void Awake()
	{
		Preload();
	}

	public void ShowNoInternetWindow(Action callback = null)
	{
		Debug.Log("No internet");
		if ((bool)window)
		{
			window.Show();
			if (callback != null)
			{
				window.SetOnRetry(callback);
			}
			else
			{
				window.SetOk();
			}
		}
	}

	private bool CheckInternet()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}
}
