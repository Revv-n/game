using System.Collections.Generic;
using System.Linq;

namespace GreenT.UI;

public class BaseWindowOpener
{
	private readonly IWindowsManager _windowsManager;

	private readonly WindowGroupID _windowID;

	private IEnumerable<IWindow> _windows;

	public BaseWindowOpener(IWindowsManager windowsManager, WindowGroupID windowID)
	{
		_windowsManager = windowsManager;
		_windowID = windowID;
	}

	public void OpenAdditive()
	{
		if (_windows == null)
		{
			_windows = _windowsManager.GetWindows(_windowID.GetWindows());
		}
		foreach (IWindow window in _windows)
		{
			window.Open();
		}
	}

	public void OpenOnly()
	{
		if (_windows == null)
		{
			_windows = _windowsManager.GetWindows(_windowID.GetWindows());
		}
		_windowsManager.OpenOnly(_windows);
	}

	public IWindow OpenAdditiveWindow()
	{
		if (_windows == null)
		{
			_windows = _windowsManager.GetWindows(_windowID.GetWindows());
		}
		foreach (IWindow window in _windows)
		{
			window.Open();
		}
		return _windows.First();
	}
}
