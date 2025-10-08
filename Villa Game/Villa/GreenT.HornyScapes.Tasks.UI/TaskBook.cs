using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.StarShop.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskBook : PopupWindow
{
	[SerializeField]
	private Button close;

	private StarShopSubwindow starShopSubWindow;

	private IDisposable closeBtnStream;

	protected override void Awake()
	{
		base.Awake();
		SubscribeOnClose();
		GetWindows();
		void GetWindows()
		{
			starShopSubWindow = windowsManager.Get<StarShopSubwindow>();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		closeBtnStream?.Dispose();
	}

	public override void Close()
	{
		base.Close();
		starShopSubWindow.Close();
	}

	private void SubscribeOnClose()
	{
		closeBtnStream = close.OnClickAsObservable().Subscribe(delegate
		{
			Close();
		});
	}
}
