using System.Collections.Generic;
using GreenT.HornyScapes.Bank.UI;
using StripClub.Model.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Shop.Offer;

public class GoToBankOfferView : LotView
{
	[SerializeField]
	private LocalizedTextMeshPro[] bundleName;

	[SerializeField]
	private LocalizedTextMeshPro[] bundleDescription;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private OpenSection sectionOpener;

	[SerializeField]
	private Transform backgroundHolder;

	private Background offerBackground;

	public override void Set(Lot lot)
	{
		base.Set(lot);
		BundleLot bundleLot = lot as BundleLot;
		sectionOpener.Set(bundleLot.GoToBankTab);
		SetupPresetData(bundleLot);
		SetupButton();
		SetupTextField(bundleName, bundleLot.NameKey);
		SetupTextField(bundleDescription, bundleLot.DescriptionKey);
	}

	protected void SetupTextField(IEnumerable<LocalizedTextMeshPro> bundleName, string key)
	{
		foreach (LocalizedTextMeshPro item in bundleName)
		{
			item.Init(key);
		}
	}

	private void SetupButton()
	{
		buyButton.onClick.RemoveListener(sectionOpener.Open);
		buyButton.onClick.AddListener(sectionOpener.Open);
	}

	private void SetupPresetData(BundleLot shopBundleLot)
	{
		if (offerBackground != null)
		{
			Object.Destroy(offerBackground.gameObject);
		}
		GameObject gameObject = shopBundleLot.Settings.GetBackground();
		_ = gameObject == null;
		gameObject = Object.Instantiate(gameObject, backgroundHolder);
		offerBackground = gameObject.GetComponent<Background>();
		if (!(offerBackground == null) && shopBundleLot.Settings.IsCharacterSpriteOverridden)
		{
			Sprite girlSprite = shopBundleLot.Settings.GetGirlSprite();
			if (!(girlSprite == null))
			{
				offerBackground.Setup(girlSprite);
			}
		}
	}
}
