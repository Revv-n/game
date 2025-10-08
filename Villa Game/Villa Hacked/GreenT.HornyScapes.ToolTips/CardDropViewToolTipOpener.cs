using StripClub.Model.Shop.UI;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class CardDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private CardDropView sourceView;

	private GirlPromoOpener girlPromoOpener;

	private int storedId;

	[Inject]
	private void InnerInit(GirlPromoOpener girlPromoOpener)
	{
		this.girlPromoOpener = girlPromoOpener;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.girl.";
		}
	}

	protected override void SetSettings()
	{
		if (storedId != sourceView.Source.ID)
		{
			base.Settings.KeyText = localizationKey + sourceView.Source.ID;
			storedId = sourceView.Source.ID;
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (sourceView.Source.IsBundleDataReady)
		{
			girlPromoOpener.TryToOpenGirlPromo(sourceView.Source);
		}
	}
}
