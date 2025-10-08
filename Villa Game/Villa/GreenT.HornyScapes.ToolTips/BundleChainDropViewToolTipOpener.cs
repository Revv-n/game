namespace GreenT.HornyScapes.ToolTips;

public class BundleChainDropViewToolTipOpener : BankInfoToolTipOpener
{
	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.bundle.chain";
		}
	}

	protected override void SetSettings()
	{
		base.Settings.KeyText = localizationKey;
	}
}
