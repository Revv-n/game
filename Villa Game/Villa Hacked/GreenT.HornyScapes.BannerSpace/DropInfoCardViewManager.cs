using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

public class DropInfoCardViewManager : ViewManager<RewardInfo, DropInfoCardView>
{
	[SerializeField]
	private RectTransform _holdContainer;

	public RectTransform HoldContainer => _holdContainer;

	public DropInfoCardView Display(RewardInfo source, RectTransform container)
	{
		DropInfoCardView dropInfoCardView = Display(source);
		dropInfoCardView.transform.SetParent(container);
		return dropInfoCardView;
	}

	public override void HideAll()
	{
		base.HideAll();
		foreach (DropInfoCardView view in views)
		{
			view.transform.SetParent(_holdContainer);
		}
	}
}
