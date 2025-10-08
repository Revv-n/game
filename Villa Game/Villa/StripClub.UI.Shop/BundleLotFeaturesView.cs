using System;
using GreenT.Localizations;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class BundleLotFeaturesView : LotFeaturesView
{
	[SerializeField]
	private GameObject sticker;

	[SerializeField]
	private GameObject[] saleObjects;

	[SerializeField]
	private TMP_Text textSticker;

	private LocalizationService _localizationService;

	private IDisposable _localizationDisposable;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(LotFeatures features)
	{
		base.Set(features);
		bool flag = features.Stickers.Contains(Stickers.Hot);
		bool flag2 = features.Stickers.Contains(Stickers.Best);
		bool active = features.Stickers.Contains(Stickers.Sale);
		sticker?.SetActive(flag || flag2);
		_localizationDisposable?.Dispose();
		string text2 = (flag ? hotStickerKey : (flag2 ? bestStickerKey : null));
		if (!string.IsNullOrEmpty(text2))
		{
			_localizationDisposable = _localizationService.ObservableText(text2).Subscribe(delegate(string text)
			{
				textSticker.text = text;
			});
		}
		else
		{
			textSticker.text = string.Empty;
		}
		GameObject[] array = saleObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(active);
		}
	}

	private void OnDisable()
	{
		_localizationDisposable?.Dispose();
	}
}
