using System;
using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats.Balance;

public class CheatBalanceViewer : MonoBehaviour
{
	[SerializeField]
	private CurrencyBalanceElement _elementPrefab;

	[SerializeField]
	private Transform _elementParent;

	[SerializeField]
	private Button _defaultValueButton;

	private List<CurrencyBalanceElement> balanceElements = new List<CurrencyBalanceElement>();

	private ICurrencyProcessor currencyProcessor;

	private GameStarter gameStarter;

	private GameSettings gameSettings;

	private CompositeDisposable disposable = new CompositeDisposable();

	[Inject]
	private void InnerInit(ICurrencyProcessor currencyProcessor, GameStarter gameStarter, GameSettings gameSettings)
	{
		this.currencyProcessor = currencyProcessor;
		this.gameStarter = gameStarter;
		this.gameSettings = gameSettings;
	}

	private void Awake()
	{
		Subscribes();
	}

	private void OnEnable()
	{
		UpdateAllCurrencySprite();
	}

	private void Subscribes()
	{
		_defaultValueButton.onClick.AddListener(SetDefaultValue);
		gameStarter.IsGameActive.ObserveEveryValueChanged((IReadOnlyReactiveProperty<bool> active) => active.Value).Subscribe(InitBalanceElements).AddTo(disposable);
	}

	private void InitBalanceElements(bool state)
	{
		if (!state)
		{
			return;
		}
		HashSet<CurrencyType> hashSet = new HashSet<CurrencyType>
		{
			CurrencyType.Message,
			CurrencyType.None,
			CurrencyType.Real
		};
		foreach (CurrencyType value in Enum.GetValues(typeof(CurrencyType)))
		{
			if (value != CurrencyType.XP && value != CurrencyType.LovePoints && value != CurrencyType.Present1 && value != CurrencyType.Present2 && value != CurrencyType.Present3 && value != CurrencyType.Present4 && !hashSet.Contains(value) && currencyProcessor.TryGetCountReactiveProperty(value, out var count))
			{
				CurrencyBalanceElement element = UnityEngine.Object.Instantiate(_elementPrefab, _elementParent);
				element.Init(GetCurrencySprite(value), value);
				balanceElements.Add(element);
				count.ObserveEveryValueChanged((IReadOnlyReactiveProperty<int> currentValue) => currentValue.Value).Pairwise().Subscribe(delegate(Pair<int> values)
				{
					UpdateCurrencyElement(element, values.Previous, values.Current);
				})
					.AddTo(disposable);
			}
		}
	}

	private void SetDefaultValue()
	{
		foreach (CurrencyBalanceElement balanceElement in balanceElements)
		{
			balanceElement.DefaultValue();
			UpdateCurrencySprite(balanceElement);
		}
	}

	private void UpdateCurrencyElement(CurrencyBalanceElement element, int previousValue, int currentValue)
	{
		int num = currentValue - previousValue;
		element.SetValue(num);
	}

	private void UpdateAllCurrencySprite()
	{
		foreach (CurrencyBalanceElement balanceElement in balanceElements)
		{
			UpdateCurrencySprite(balanceElement);
		}
	}

	private void UpdateCurrencySprite(CurrencyBalanceElement element)
	{
		Sprite currencySprite = GetCurrencySprite(element.CurrencyType);
		element.UpdateSprite(currencySprite);
	}

	private Sprite GetCurrencySprite(CurrencyType currencyType)
	{
		return gameSettings.CurrencySettings[currencyType, default(CompositeIdentificator)].Sprite;
	}

	private void OnDestroy()
	{
		_defaultValueButton.onClick.RemoveAllListeners();
		disposable.Dispose();
	}
}
