using System;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Presents.Models;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentDropView : MonoView<Present>
{
	public class Manager : ViewManager<PresentDropView>
	{
	}

	[SerializeField]
	private TextMeshProUGUI _presentsCount;

	[SerializeField]
	private Image _presentIcon;

	private GameSettings _gameSettings;

	private PresentsManager _presentsManager;

	[Inject]
	private void Init(GameSettings gameSettings, PresentsManager presentsManager)
	{
		_gameSettings = gameSettings;
		_presentsManager = presentsManager;
	}

	public void Set(CurrencySelector selector, int quantity)
	{
		_presentsCount.text = quantity.ToString();
		string presentId = GetPresentId(selector.Currency);
		Present source = _presentsManager.Get(presentId);
		Set(source);
		SimpleCurrency.CurrencyKey key = new SimpleCurrency.CurrencyKey(selector.Currency, selector.Identificator);
		_presentIcon.sprite = _gameSettings.CurrencySettings[key].Sprite;
	}

	private string GetPresentId(CurrencyType currencyType)
	{
		string text = "present_";
		return currencyType switch
		{
			CurrencyType.Present1 => text + "1", 
			CurrencyType.Present2 => text + "2", 
			CurrencyType.Present3 => text + "3", 
			CurrencyType.Present4 => text + "4", 
			_ => throw new Exception("Invalid present id (" + text + ")"), 
		};
	}
}
