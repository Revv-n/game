using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Bank.UI;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Model.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop.Offer;

public class DateGoToBankOfferView : LotView
{
	[SerializeField]
	private LocalizedTextMeshPro[] _bundleName;

	[SerializeField]
	private TMP_Text _bundleDescription1;

	[SerializeField]
	private TMP_Text _bundleDescription2;

	[SerializeField]
	private TMP_Text _bundleDescription3;

	[SerializeField]
	private Button _buyButton;

	[SerializeField]
	private OpenSection _sectionOpener;

	[SerializeField]
	private Transform _backgroundHolder;

	private IWindowsManager _windowsManager;

	private LocalizationService _localizationService;

	private OfferWindow _offerWindow;

	private Background _offerBackground;

	[Inject]
	private void Init(IWindowsManager windowsManager, LocalizationService localizationService)
	{
		_windowsManager = windowsManager;
		_localizationService = localizationService;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		BundleLot bundleLot = lot as BundleLot;
		_sectionOpener.Set(bundleLot.GoToBankTab);
		SetupPresetData(bundleLot);
		SetupButton();
		SetupTextField(_bundleName, bundleLot.NameKey);
		string[] array = _localizationService.Text(bundleLot.DescriptionKey).Split('|', StringSplitOptions.None);
		if (array.Length == 3)
		{
			_bundleDescription1.text = _localizationService.Text(array[0]);
			_bundleDescription2.text = _localizationService.Text(array[1]);
			_bundleDescription3.text = _localizationService.Text(array[2]);
		}
		else if (array.Length == 1)
		{
			_bundleDescription1.text = _localizationService.Text(array[0]);
			_bundleDescription2.text = string.Empty;
			_bundleDescription3.text = string.Empty;
		}
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
		_buyButton.onClick.RemoveListener(OnBuyButtonClicked);
		_buyButton.onClick.AddListener(OnBuyButtonClicked);
	}

	private void SetupPresetData(BundleLot shopBundleLot)
	{
		if (_offerBackground != null)
		{
			UnityEngine.Object.Destroy(_offerBackground.gameObject);
		}
		GameObject obj = shopBundleLot.Settings.GetBackground();
		_ = obj == null;
		if (UnityEngine.Object.Instantiate(obj, _backgroundHolder).TryGetComponent<Background>(out _offerBackground) && shopBundleLot.Settings.IsCharacterSpriteOverridden)
		{
			Sprite girlSprite = shopBundleLot.Settings.GetGirlSprite();
			if (!(girlSprite == null))
			{
				_offerBackground.Setup(girlSprite);
			}
		}
	}

	private void OnBuyButtonClicked()
	{
		if (_offerWindow == null)
		{
			_offerWindow = _windowsManager.Get<OfferWindow>();
		}
		_offerWindow.Close();
		_sectionOpener.Open();
	}
}
