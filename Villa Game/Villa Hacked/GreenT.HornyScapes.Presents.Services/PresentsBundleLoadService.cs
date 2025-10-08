using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Presents.Managers;
using GreenT.HornyScapes.Presents.Models;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Presents.Services;

public class PresentsBundleLoadService
{
	private readonly PresentsManager _presentsManager;

	private readonly ILoader<string, PresentBundleData> _presentsBundleDataLoader;

	private readonly PresentBundleManager _presentBundleManager;

	private readonly GameSettings _gameSettings;

	public PresentsBundleLoadService(PresentsManager presentsManager, ILoader<string, PresentBundleData> presentsBundleDataLoader, PresentBundleManager presentBundleManager, GameSettings gameSettings)
	{
		_presentsManager = presentsManager;
		_presentsBundleDataLoader = presentsBundleDataLoader;
		_presentBundleManager = presentBundleManager;
		_gameSettings = gameSettings;
	}

	public IObservable<Unit> LoadBundles()
	{
		return Observable.Select<PresentBundleData, Unit>(Observable.LastOrDefault<PresentBundleData>(Observable.SelectMany<Present, PresentBundleData>(Observable.ToObservable<Present>(_presentsManager.Collection), (Func<Present, IObservable<PresentBundleData>>)Load)), (Func<PresentBundleData, Unit>)((PresentBundleData _) => Unit.Default));
	}

	private IObservable<PresentBundleData> Load(Present present)
	{
		return Observable.Do<PresentBundleData>(_presentsBundleDataLoader.Load(present.Id), (Action<PresentBundleData>)delegate(PresentBundleData bundle)
		{
			UpdatePresentSettings(present, bundle);
			present.SetIcon(bundle.Sprite);
			_presentBundleManager.Add(bundle);
		});
	}

	private void UpdatePresentSettings(Present present, PresentBundleData bundle)
	{
		if (TryGetCurrencyType(present, out var currencyType))
		{
			CompositeIdentificator identificator = new CompositeIdentificator(default(int));
			SimpleCurrency.CurrencyKey currencyKey = new SimpleCurrency.CurrencyKey(currencyType, identificator);
			if (!_gameSettings.CurrencySettings.Contains(currencyType, identificator))
			{
				CurrencySettings currencySettings = new CurrencySettings();
				((Dictionary<SimpleCurrency.CurrencyKey, CurrencySettings>)_gameSettings.CurrencySettings).TryAdd(currencyKey, currencySettings);
			}
			_gameSettings.CurrencySettings[currencyKey].SetSprite(bundle.Sprite);
			_gameSettings.CurrencySettings[currencyKey].SetAlternativeSprite(bundle.Sprite);
			_gameSettings.CurrencySettings[currencyKey].SetLocalization(bundle.CurrencyKey);
		}
	}

	private bool TryGetCurrencyType(Present present, out CurrencyType currencyType)
	{
		currencyType = CurrencyType.None;
		if (!int.TryParse(present.Id.Replace("present_", ""), out var result))
		{
			return false;
		}
		currencyType = result switch
		{
			1 => CurrencyType.Present1, 
			2 => CurrencyType.Present2, 
			3 => CurrencyType.Present3, 
			4 => CurrencyType.Present4, 
			_ => throw new Exception("Invalid present id (" + present.Id + ")"), 
		};
		return true;
	}
}
