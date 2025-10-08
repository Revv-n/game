using StripClub.Model;
using StripClub.Model.Shop.UI;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ResourceDropCardToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private ResourceDropCardBigView sourceView;

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
		CurrencyType sourceCurrency = sourceView.SourceCurrency;
		if (storedCurrencyType != sourceCurrency)
		{
			base.Settings.KeyText = localizationKey + sourceCurrency.ToString().ToLower() + $".{sourceView.SourceIdentificator[0]}";
			storedCurrencyType = sourceCurrency;
		}
	}
}
