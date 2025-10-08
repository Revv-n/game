using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class TutorialWindow : PopupWindow
{
	[SerializeField]
	private BlinkText blinkText;

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		blinkText.Init();
	}

	public override void Open()
	{
		base.Open();
		blinkText.Play();
	}

	public override void Close()
	{
		blinkText.Stop(base.Close);
	}
}
