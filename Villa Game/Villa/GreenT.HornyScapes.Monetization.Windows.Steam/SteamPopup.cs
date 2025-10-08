namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class SteamPopup : MonetizationPopup
{
	protected override void Awake()
	{
		base.Awake();
		SupportButton.onClick.AddListener(supportUrlOpener.OpenUrl);
		AbortButton.onClick.AddListener(Close);
	}
}
