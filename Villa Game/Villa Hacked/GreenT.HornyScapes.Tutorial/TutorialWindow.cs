using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialWindow : Window
{
	[SerializeField]
	private Image backBlocker;

	[SerializeField]
	private Image blurBackBlocker;

	[SerializeField]
	private GameObject settings;

	[SerializeField]
	private DynamicHoleMask mask;

	public void Open(TutorialStepSO step)
	{
		backBlocker.gameObject.SetActive(step.BlockScreen);
		blurBackBlocker.gameObject.SetActive(step.UseBlur);
		settings.SetActive(step.ShowSettings);
		TryShowMask(step);
		Open();
	}

	public override void Close()
	{
		base.Close();
		backBlocker.gameObject.SetActive(value: false);
		blurBackBlocker.gameObject.SetActive(value: false);
		mask.Hide();
	}

	private void TryShowMask(TutorialStepSO step)
	{
		if (step.UseMask)
		{
			mask.Show(step.MaskSettings);
		}
		else
		{
			mask.Hide();
		}
	}
}
