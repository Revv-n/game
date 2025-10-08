using GreenT.HornyScapes.Bank.UI;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class CurrencyDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private CurrencyDropView sourceView;

	private CurrencyType storedCurrencyType = CurrencyType.None;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.resource.";
		}
	}

	protected override void SetSettings()
	{
		if (storedCurrencyType != sourceView.CurrencyType)
		{
			base.Settings.KeyText = localizationKey + sourceView.CurrencyType.ToString().ToLower() + $".{sourceView.CompositeIdentificator[0]}";
			storedCurrencyType = sourceView.CurrencyType;
		}
	}
}
