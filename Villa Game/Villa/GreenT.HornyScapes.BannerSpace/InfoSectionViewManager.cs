using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class InfoSectionViewManager : ViewManager<RewardInfo[], InfoSectionView>
{
	private DropInfoCardViewManager _dropInfoCardViewManager;

	[Inject]
	public void Initialization(DropInfoCardViewManager dropInfoCardViewManager)
	{
		_dropInfoCardViewManager = dropInfoCardViewManager;
	}

	public InfoSectionView Display(RewardInfo[] source, RectTransform container)
	{
		InfoSectionView infoSectionView = Display(source);
		infoSectionView.transform.SetParent(container);
		return infoSectionView;
	}

	public override void HideAll()
	{
		_dropInfoCardViewManager.HideAll();
		base.HideAll();
	}
}
