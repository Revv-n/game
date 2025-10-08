using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Windows;

public class DateLoadingWindow : Window
{
	[SerializeField]
	private Animation _loadingAnimation;

	public void PlayAnimations()
	{
		_loadingAnimation.ResetToAnimStart();
		_loadingAnimation.Play();
	}

	public void StopAnimations()
	{
		_loadingAnimation.Stop();
	}
}
