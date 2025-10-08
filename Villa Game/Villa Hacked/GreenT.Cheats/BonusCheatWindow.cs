using System;
using System.Collections.Generic;
using GreenT.Bonus;
using GreenT.HornyScapes.GameItems;
using GreenT.Multiplier;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.Cheats;

public class BonusCheatWindow : MonoBehaviour
{
	[Inject]
	private CardsCollection cardsCollection;

	[SerializeField]
	private BonusesCheatDictionary bonusesViews = new BonusesCheatDictionary();

	[Inject]
	private MultiplierManager multiplierManager;

	[Inject]
	private BonusManager bonusManager;

	[Inject]
	private GameItemConfigManager gameItemConfigManager;

	public void InitBonuses()
	{
		AdjustedMultiplierDictionary<int, SummingCompositeMultiplier> spawnerMaxAmountMultipliers = multiplierManager.SpawnerMaxAmountMultipliers;
		RefreshMultiplierView("MaxAmount", spawnerMaxAmountMultipliers, BonusType.increaseDropValue);
		SubscribeOnChangeBonus("MaxAmount", spawnerMaxAmountMultipliers, BonusType.increaseDropValue);
		SubscribeBonusBtn(spawnerMaxAmountMultipliers, BonusType.increaseDropValue);
		AdjustedMultiplierDictionary<int, SummingCompositeMultiplier> spawnerProductionMultipliers = multiplierManager.SpawnerProductionMultipliers;
		RefreshMultiplierView("Production", spawnerProductionMultipliers, BonusType.increaseProductionValue);
		SubscribeOnChangeBonus("Production", spawnerProductionMultipliers, BonusType.increaseProductionValue);
		SubscribeBonusBtn(spawnerProductionMultipliers, BonusType.increaseProductionValue);
		AdjustedMultiplierDictionary<int, MultiplyingCompositeMultiplier> spawnerReloadMultipliers = multiplierManager.SpawnerReloadMultipliers;
		RefreshMultiplierView("Reload", spawnerReloadMultipliers, BonusType.decreaseReloadTime);
		SubscribeOnChangeBonus("Reload", spawnerReloadMultipliers, BonusType.decreaseReloadTime);
		SubscribeBonusBtn(spawnerReloadMultipliers, BonusType.decreaseReloadTime);
	}

	private void SubscribeBonusBtn<K>(AdjustedMultiplierDictionary<int, K> multiplier, BonusType bonusType) where K : ICompositeMultiplier, new()
	{
		bonusesViews[bonusType].InputField.onValueChanged.AddListener(delegate(string _strValue)
		{
			if (int.TryParse(_strValue, out var result2))
			{
				bonusesViews[bonusType].ShowInfo.interactable = gameItemConfigManager.TryGetConfig(result2, out var _);
			}
		});
		bonusesViews[bonusType].ShowInfo.onClick.AddListener(delegate
		{
			int.TryParse(bonusesViews[bonusType].InputField.text, out var _);
		});
	}

	private string GetInfluencedItems(BonusType bonusType)
	{
		string text = string.Empty;
		foreach (ISimpleBonus item in bonusManager.Collection)
		{
			ITypeBonus typeBonus = (ITypeBonus)item;
			if (item.IsApplied && typeBonus.BonusType == bonusType && item is MultiplierBonus multiplierBonus)
			{
				text += $"{bonusManager.GetParent(multiplierBonus)} - {multiplierBonus.Multiplier.Factor.Value} ";
			}
		}
		return text;
	}

	private void SubscribeOnChangeBonus<K>(string name, AdjustedMultiplierDictionary<int, K> multiplier, BonusType bonusType) where K : ICompositeMultiplier, new()
	{
		ObservableExtensions.Subscribe<double>((IObservable<double>)multiplier.Total.Factor, (Action<double>)delegate
		{
			RefreshMultiplierView(name, multiplier, bonusType);
		});
		ObservableExtensions.Subscribe<int>(multiplier.OnAdded, (Action<int>)delegate
		{
			RefreshMultiplierView(name, multiplier, bonusType);
		});
	}

	private void RefreshMultiplierView<K>(string name, AdjustedMultiplierDictionary<int, K> multiplier, BonusType bonusType) where K : ICompositeMultiplier, new()
	{
		bonusesViews[bonusType].Text.text = name + "\n" + $"Total: {multiplier.Total.Factor.Value} Affected IDs: {GetIDs(multiplier.Keys())}";
	}

	private string GetIDs(IEnumerable<int> keys)
	{
		string text = string.Empty;
		foreach (int key in keys)
		{
			text += $"{key}, ";
		}
		return text;
	}

	private void OnDestroy()
	{
		foreach (BonusCheatView value in bonusesViews.Values)
		{
			value.InputField.onValueChanged.RemoveAllListeners();
			value.ShowInfo.onClick.RemoveAllListeners();
		}
	}
}
