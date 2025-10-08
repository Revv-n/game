using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.Localizations;

public class LocalizedLogo : MonoBehaviour
{
	[Inject]
	private LocalizationState _localizationState;

	[SerializeField]
	private Image _logoImg;

	[SerializeField]
	private List<LocalizedLogoDictionaryObject> _logoSpriteDict;

	private IDisposable _localizationDownloadStream;

	private void OnEnable()
	{
		ChangeLogoLanguage(_localizationState.CurrentLanguage);
		_localizationDownloadStream = _localizationState.OnLanguageChange.Subscribe(ChangeLogoLanguage, delegate(Exception exception)
		{
			exception.LogException();
		});
	}

	private void ChangeLogoLanguage(string language)
	{
		string text = language;
		if (!(text == "ZH-HANS-CN"))
		{
			if (text == "ZH-HANS-TW")
			{
				_logoImg.sprite = _logoSpriteDict.FirstOrDefault((LocalizedLogoDictionaryObject x) => x.Language == SystemLanguage.ChineseTraditional).LogoSprite;
				return;
			}
			Sprite sprite = _logoSpriteDict.FirstOrDefault((LocalizedLogoDictionaryObject x) => x.Language.ToString() == language)?.LogoSprite;
			if ((bool)sprite)
			{
				_logoImg.sprite = sprite;
			}
		}
		else
		{
			_logoImg.sprite = _logoSpriteDict.FirstOrDefault((LocalizedLogoDictionaryObject x) => x.Language == SystemLanguage.ChineseSimplified).LogoSprite;
		}
	}

	private void OnDisable()
	{
		_localizationDownloadStream.Dispose();
	}
}
