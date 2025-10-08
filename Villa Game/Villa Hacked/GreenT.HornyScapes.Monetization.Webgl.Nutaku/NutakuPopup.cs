using Merge;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class NutakuPopup : MonetizationPopup
{
	public override void Open()
	{
		base.Open();
		AbortButton.SetActive(active: false);
	}

	public override void SetFailedView()
	{
		base.SetFailedView();
		SupportButton.SetActive(active: false);
	}
}
