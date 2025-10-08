using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class JewelResourceWindow : WindowAnimatedByStrategy
{
	[SerializeField]
	private JewelsCurrencyView _jewelsCurrencyView;

	public override void Open()
	{
		if (_jewelsCurrencyView.IsAboveZero)
		{
			base.Open();
		}
	}

	public void ForceOpen()
	{
		base.Open();
	}
}
