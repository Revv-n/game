using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentDropViewTooltipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private PresentDropView _presentDropView;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.";
		}
	}

	protected override void SetSettings()
	{
		string text = localizationKey + _presentDropView.Source.Id;
		base.Settings.KeyText = text.ToLower();
	}
}
