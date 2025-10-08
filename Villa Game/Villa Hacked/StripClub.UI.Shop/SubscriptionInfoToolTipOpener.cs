using GreenT.HornyScapes.ToolTips;

namespace StripClub.UI.Shop;

public class SubscriptionInfoToolTipOpener : BankInfoToolTipOpener
{
	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.subscription.{0}.desc";
		}
	}

	protected override void SetSettings()
	{
		base.Settings.KeyText = localizationKey;
	}
}
