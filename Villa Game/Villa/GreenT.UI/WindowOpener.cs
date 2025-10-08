using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GreenT.UI;

public class WindowOpener : MonoBehaviour
{
	[SerializeField]
	protected WindowGroupID[] windowsGroup;

	[SerializeField]
	protected bool isOpenOnly;

	protected IEnumerable<IWindow> windows;

	protected IWindowsManager WindowsOpener { get; private set; }

	public IEnumerable<IWindow> Windows => GetTargetWindows();

	[Inject]
	private void InnerInit(IWindowsManager windowsManager)
	{
		WindowsOpener = windowsManager;
	}

	public void Set(WindowGroupID[] windowsGroup)
	{
		this.windowsGroup = windowsGroup;
		windows = null;
	}

	private IEnumerable<IWindow> GetTargetWindows()
	{
		if (WindowsOpener == null)
		{
			WindowsOpener = Object.FindObjectOfType<UIManager>();
		}
		if (windows == null)
		{
			List<WindowID> list = new List<WindowID>();
			WindowGroupID[] array = windowsGroup;
			foreach (WindowGroupID windowGroupID in array)
			{
				list.AddRange(windowGroupID.GetWindows());
			}
			windows = WindowsOpener.GetWindows(list.ToArray());
		}
		return windows;
	}

	public virtual void ClickSelector()
	{
		if (isOpenOnly)
		{
			OpenOnly();
		}
		else
		{
			Click();
		}
	}

	public virtual void Click()
	{
		foreach (IWindow targetWindow in GetTargetWindows())
		{
			targetWindow.Open();
		}
	}

	public virtual void Close()
	{
		foreach (IWindow targetWindow in GetTargetWindows())
		{
			targetWindow.Close();
		}
	}

	public virtual void OpenOnly()
	{
		WindowsOpener.OpenOnly(GetTargetWindows());
	}
}
