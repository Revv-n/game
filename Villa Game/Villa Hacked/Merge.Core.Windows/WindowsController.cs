using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Merge.Core.Windows;

public class WindowsController : Controller<WindowsController>, IInputBlocker
{
	[SerializeField]
	private PublicBackground background;

	[SerializeField]
	private Transform root;

	private List<Window> allWindows = new List<Window>();

	private List<Window> showStack = new List<Window>();

	public Window ActiveWindow { get; private set; }

	public bool HasActiveWindow => ActiveWindow != null;

	public bool HasWindowInStack => showStack.Count > 0;

	bool IInputBlocker.BlocksClick => HasActiveWindow;

	public bool CloseByBackgroundEnabled { get; set; } = true;


	public override void Preload()
	{
		base.Preload();
		FindAllWindows();
		SubscribeWindows();
		background.OnClick += AtBackgroundClick;
	}

	private void FindAllWindows()
	{
		for (int i = 0; i < root.childCount; i++)
		{
			Window component = root.GetChild(i).GetComponent<Window>();
			if (!(component == null))
			{
				allWindows.Add(component);
			}
		}
	}

	private void SubscribeWindows()
	{
		foreach (Window allWindow in allWindows)
		{
			allWindow.OnBecomeShow += AtWindowBecomeShow;
			allWindow.OnBeckomeHide += AtWindowBecomeHide;
			allWindow.OnEndHide += AtWindowEndHide;
		}
	}

	private void AtWindowBecomeShow(Window sender)
	{
		if (HasActiveWindow)
		{
			ActiveWindow.TempHide(Unpause);
			showStack.Add(ActiveWindow);
			sender.ShowTween.Pause();
			sender.SetActive(active: false);
		}
		background.DoAlpha(sender.WindowConfig.BackAlpha, !HasActiveWindow);
		ActiveWindow = sender;
		background.AllowCloseClick = sender.WindowConfig.AllowBackClose;
		void Unpause()
		{
			sender.SetActive(active: true);
			sender.ShowTween.Play();
		}
	}

	private void AtWindowBecomeHide(Window sender)
	{
		if (HasWindowInStack)
		{
			if (showStack.Last() == null)
			{
				showStack.Remove(showStack.Last());
				background.DoHide(sender.WindowConfig.ShowTime);
			}
			else
			{
				Window window = showStack.Last();
				background.DoAlpha(window.WindowConfig.BackAlpha, fromZero: false);
				background.AllowCloseClick = window.WindowConfig.AllowBackClose;
			}
		}
		else
		{
			background.DoHide(sender.WindowConfig.ShowTime);
		}
	}

	private void AtWindowEndHide(Window sender)
	{
		if (HasWindowInStack)
		{
			ActiveWindow = showStack.Last();
			ActiveWindow.TempUnhide(null);
			showStack.Remove(ActiveWindow);
		}
		else
		{
			ActiveWindow = null;
		}
	}

	private void AtBackgroundClick()
	{
		if (CloseByBackgroundEnabled && !(ActiveWindow == null) && !ActiveWindow.IsPlayingShowTween)
		{
			ActiveWindow.Hide();
		}
	}
}
