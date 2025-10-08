using System;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Extensions;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPreview : MonoView<OfferSettings>
{
	[SerializeField]
	protected OfferPreviewRegion displayRegion;

	[SerializeField]
	private TMP_Text[] titleTextFields;

	[SerializeField]
	protected Image background;

	[SerializeField]
	protected Image icon;

	[SerializeField]
	private MonoTimer timer;

	[SerializeField]
	private GameObject sticker;

	[SerializeField]
	private TMP_Text saleValue;

	[SerializeField]
	private WindowOpener windowOpener;

	protected BundlesProviderBase bundlesProvider;

	private SectionController sectionController;

	private TimeHelper timeHelper;

	private LocalizationService _localizationService;

	private IDisposable localizationStream;

	[Inject]
	public void Init(BundlesProviderBase bundlesProvider, SectionController sectionController, TimeHelper timeHelper, LocalizationService localizationService)
	{
		this.bundlesProvider = bundlesProvider;
		this.sectionController = sectionController;
		this.timeHelper = timeHelper;
		_localizationService = localizationService;
	}

	public override void Set(OfferSettings offer)
	{
		base.gameObject.name = offer.ID.ToString();
		base.Set(offer);
		SetupSticker(offer);
		SetupTimer(offer);
		SetTitle(offer);
		ViewPreset preset = GetPreset();
		Sprite sprite = SelectIcon(preset);
		SetIcon(sprite);
		Sprite sprite2 = SelectBackground(offer.ViewSets, preset);
		SetBackground(sprite2);
	}

	private void SetTitle(OfferSettings offer)
	{
		if (titleTextFields != null)
		{
			localizationStream?.Dispose();
			localizationStream = _localizationService.ObservableText(offer.Bundles[0].NameKey).Subscribe(SetForEachTextField);
		}
		void SetForEachTextField(string text)
		{
			TMP_Text[] array = titleTextFields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = text;
			}
		}
	}

	protected virtual void SetIcon(Sprite icon)
	{
		this.icon.sprite = icon;
	}

	protected virtual void SetBackground(Sprite background)
	{
		this.background.sprite = background;
	}

	protected virtual Sprite SelectBackground(OfferSettings.ViewSettings viewSets, ViewPreset preset)
	{
		OfferPreviewBackgroundPreset offerPreviewBackgroundPreset = null;
		if (!string.IsNullOrEmpty(viewSets.BackgroundKey))
		{
			offerPreviewBackgroundPreset = bundlesProvider.TryFindInConcreteBundle<OfferPreviewBackgroundPreset>(ContentSource.Default, viewSets.BackgroundKey);
		}
		if (offerPreviewBackgroundPreset != null)
		{
			return offerPreviewBackgroundPreset[displayRegion];
		}
		return preset.Background[displayRegion];
	}

	protected virtual Sprite SelectIcon(ViewPreset preset)
	{
		return preset.Icon[displayRegion];
	}

	private void SetupTimer(OfferSettings offer)
	{
		timer.Init(offer.DisplayTimeLocker.Timer, timeHelper.UseCombineFormat);
	}

	private void SetupSticker(OfferSettings offer)
	{
		BundleLot bundleLot = offer.Bundles[offer.Bundles.Length - 1];
		sticker.SetActive(bundleLot.Features.SaleValue.HasValue);
		if (bundleLot.Features.SaleValue.HasValue)
		{
			int value = bundleLot.Features.SaleValue.Value;
			saleValue.text = value + "%";
		}
	}

	protected ViewPreset GetPreset()
	{
		if (base.Source?.ViewSets == null)
		{
			new Exception($"Не заданы данные Source = {base.Source} | Source.ViewSets = {base.Source?.ViewSets}").LogException();
			return null;
		}
		return base.Source.ViewSets.GetPreset();
	}

	public void DisplayContent()
	{
		windowOpener.Click();
		sectionController.LoadSection(base.Source);
		base.Source.InvokeOnClickedEvent();
	}

	protected virtual void OnDisable()
	{
		localizationStream?.Dispose();
	}
}
