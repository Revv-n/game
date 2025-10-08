using GreenT.HornyScapes.Collections.Promote.UI.Animation;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class AnimatedPromoteCardViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private AnimatedPromoteCardView sourceView;

	private GirlPromoOpener girlPromoOpener;

	[Inject]
	private void InnerInit(GirlPromoOpener girlPromoOpener)
	{
		this.girlPromoOpener = girlPromoOpener;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		girlPromoOpener.TryToOpenGirlPromo(sourceView.Source);
	}
}
