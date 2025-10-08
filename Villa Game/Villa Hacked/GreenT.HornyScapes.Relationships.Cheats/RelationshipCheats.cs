using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Cheats;

public class RelationshipCheats : MonoBehaviour, IDisposable
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _increaseLovePointsButton;

	private ICurrencyProcessor _currencyProcessor;

	[Inject]
	private void Init(ICurrencyProcessor currencyProcessor)
	{
		_currencyProcessor = currencyProcessor;
	}

	private void Start()
	{
		_increaseLovePointsButton.onClick.AddListener(IncreaseLovePoints);
	}

	private void IncreaseLovePoints()
	{
		IncreaseRelationshipPoints(CurrencyType.LovePoints);
	}

	private void IncreaseRelationshipPoints(CurrencyType pointsType)
	{
		string text = _inputField.text;
		try
		{
			string[] array = text.Split(':', StringSplitOptions.None);
			int num = int.Parse(array[0]);
			int amount = int.Parse(array[1]);
			_currencyProcessor.TryAdd(pointsType, amount, CurrencyAmplitudeAnalytic.SourceType.Relationship, new CompositeIdentificator(num));
		}
		catch (Exception ex)
		{
			ex.SendException("Can't parse cheat input string " + text + ". It must be id:amount. Exception: " + ex.Message);
		}
	}

	private void OnDestroy()
	{
		Dispose();
	}

	public void Dispose()
	{
		_increaseLovePointsButton.onClick.RemoveListener(IncreaseLovePoints);
	}
}
