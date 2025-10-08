using System;
using System.Linq;
using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class InfoWindow : MonoView<Banner>
{
	[Header("Info Button")]
	[SerializeField]
	private Button _openButton;

	[SerializeField]
	private TMP_Text _detailsText;

	[SerializeField]
	private TMP_Text _closeText;

	[SerializeField]
	private TMP_Text _infoText;

	private InfoSectionViewManager _infoSectionViewManager;

	private LocalizationService _localizationService;

	private IDisposable _clickStream;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	public void Initialization(InfoSectionViewManager infoSectionViewManager, LocalizationService localizationService)
	{
		_infoSectionViewManager = infoSectionViewManager;
		_localizationService = localizationService;
	}

	public override void Set(Banner source)
	{
		_clickStream?.Dispose();
		_clickStream = _openButton.onClick.AsObservable().Subscribe(delegate
		{
			ButtonClick();
		});
		base.Set(source);
		_infoSectionViewManager.HideAll();
		if (source.LegendaryRewardInfos.Any())
		{
			_infoSectionViewManager.Display(source.LegendaryRewardInfos);
		}
		if (source.EpicRewardInfos.Any())
		{
			_infoSectionViewManager.Display(source.EpicRewardInfos);
		}
		if (source.RareRewardInfos.Any())
		{
			_infoSectionViewManager.Display(source.RareRewardInfos);
		}
		_localizationDisposables.Clear();
		string key = $"ui.shop.banner_{base.Source.Id}.descr";
		_localizationService.ObservableText(key).Subscribe(delegate(string text)
		{
			_infoText.text = text;
		}).AddTo(_localizationDisposables);
	}

	public override void Display(bool display)
	{
		base.Display(display);
		if (display)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void ButtonClick()
	{
		if (base.gameObject.activeSelf)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	private void Show()
	{
		_detailsText.gameObject.SetActive(value: false);
		_closeText.gameObject.SetActive(value: true);
		base.gameObject.SetActive(value: true);
	}

	private void Hide()
	{
		_localizationDisposables.Clear();
		_detailsText.gameObject.SetActive(value: true);
		_closeText.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_localizationDisposables?.Dispose();
	}
}
