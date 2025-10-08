using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class WindowAnimatedByStrategy : Window
{
	[SerializeField]
	private MonoDisplayStrategy displayStrategy;

	protected override void SetActive(bool isActive)
	{
		displayStrategy.Display(isActive);
		IsOpened = isActive;
	}
}
