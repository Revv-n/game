using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class TaskJewelAnimateBezier : TaskBaseAnimateBezier<JewelAnimController>
{
	private IWindowsManager _windowsManager;

	private JewelResourceWindow _jewelResourceWindow;

	[Inject]
	private void Init(IWindowsManager windowsManager)
	{
		_windowsManager = windowsManager;
	}

	protected override void AnimationBefore()
	{
		if (_jewelResourceWindow == null)
		{
			_jewelResourceWindow = _windowsManager.Get<JewelResourceWindow>();
		}
		_jewelResourceWindow.ForceOpen();
	}
}
