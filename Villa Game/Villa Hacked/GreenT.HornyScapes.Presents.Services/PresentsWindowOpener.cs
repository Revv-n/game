using GreenT.HornyScapes.Presents.UI;
using GreenT.UI;

namespace GreenT.HornyScapes.Presents.Services;

public class PresentsWindowOpener
{
	private PresentsWindow _presentsWindow;

	private readonly IWindowsManager _windowsManager;

	private readonly PresentsManager _presentsManager;

	public PresentsWindowOpener(IWindowsManager windowsManager, PresentsManager presentsManager)
	{
		_windowsManager = windowsManager;
		_presentsManager = presentsManager;
	}

	public void Open()
	{
		if (_presentsWindow == null)
		{
			_presentsWindow = _windowsManager.Get<PresentsWindow>();
		}
		_windowsManager.Open(_presentsWindow);
		_presentsWindow.PresentsWindowSetter.Set(_presentsManager.Collection);
	}
}
