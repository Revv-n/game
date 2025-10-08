using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Localizations.Data;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.Localizations;

public class LocalizationProvider : IInitializable, IDisposable
{
	private LocalizationDictionary _currentLocalization;

	private LocalizationDictionary _defaultLocalization;

	private readonly LocalizationVariantsProvider _variantsProvider;

	private readonly LocalizationLoader _localizationLoader;

	private readonly ScriptableObjectDB _localLocalization;

	private readonly LocalizationVartiantsStructureInitializer _localizationVartiantsStructureInitializer;

	private readonly LocalizationState _localizationState;

	private readonly Dictionary<string, LocalizationDictionary> _localizationsDictionary = new Dictionary<string, LocalizationDictionary>();

	private readonly Subject<LocalizationDictionary> _onLocalizationChange = new Subject<LocalizationDictionary>();

	private IDisposable _localizationDownloadStream;

	public IObservable<LocalizationDictionary> OnLocalizationChange => Observable.AsObservable<LocalizationDictionary>((IObservable<LocalizationDictionary>)_onLocalizationChange);

	public IEnumerable<string> LocalizationKeys => _currentLocalization.Keys;

	public LocalizationProvider(LocalizationState localizationState, LocalizationVariantsProvider variantsProvider, LocalizationLoader localizationLoader, LocalizationVartiantsStructureInitializer vartiantsStructureInitializer, ScriptableObjectDB localLocalization)
	{
		_localizationState = localizationState;
		_variantsProvider = variantsProvider;
		_localizationLoader = localizationLoader;
		_localLocalization = localLocalization;
		_localizationVartiantsStructureInitializer = vartiantsStructureInitializer;
	}

	public void Initialize()
	{
		SetDefaultLocalization();
		_localizationDownloadStream = ObservableExtensions.Subscribe<LocalizationDictionary>(Observable.SelectMany<string, LocalizationDictionary>(_localizationState.OnLanguageChange, (Func<string, IObservable<LocalizationDictionary>>)GetLocalization), (Action<LocalizationDictionary>)SetCurrentLocalization, (Action<Exception>)delegate(Exception exception)
		{
			exception.LogException();
		});
	}

	private void SetDefaultLocalization()
	{
		ObservableExtensions.Subscribe<LocalizationDictionary>(Observable.Do<LocalizationDictionary>(Observable.Take<LocalizationDictionary>(Observable.SelectMany<string, LocalizationDictionary>(Observable.Select<LocalizationVariant, string>(Observable.Where<LocalizationVariant>(Observable.Select<bool, LocalizationVariant>(Observable.Where<bool>((IObservable<bool>)_localizationVartiantsStructureInitializer.IsInitialized, (Func<bool, bool>)((bool isInit) => isInit && _defaultLocalization == null)), (Func<bool, LocalizationVariant>)((bool _) => _variantsProvider.Collection.FirstOrDefault((LocalizationVariant step) => step.IsDefault))), (Func<LocalizationVariant, bool>)((LocalizationVariant variant) => variant != null)), (Func<LocalizationVariant, string>)((LocalizationVariant variant) => variant.Key)), (Func<string, IObservable<LocalizationDictionary>>)GetLocalization), 1), (Action<LocalizationDictionary>)delegate
		{
			SendUpdateLocalization();
		}));
	}

	public string TryGetValue(string key)
	{
		if (_currentLocalization != null && _currentLocalization.TryGetValue(key.ToLowerInvariant(), out var value) && !string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (_defaultLocalization != null && _defaultLocalization.TryGetValue(key.ToLowerInvariant(), out value))
		{
			return value;
		}
		return key;
	}

	private IObservable<LocalizationDictionary> GetLocalization(string language)
	{
		if (!_localizationVartiantsStructureInitializer.IsInitialized.Value)
		{
			return Observable.Return<LocalizationDictionary>(GetLocalLocalization(language));
		}
		return GetRemoteLocalization(language);
	}

	private LocalizationDictionary GetLocalLocalization(string language)
	{
		if (_localLocalization.localization.TryGetValue(language, out var value))
		{
			value.IsLocal = true;
			_localizationsDictionary.Add(language, value);
			_currentLocalization = value;
			return value;
		}
		KeyValuePair<string, LocalizationDictionary> keyValuePair = _localLocalization.localization.First();
		keyValuePair.Value.IsLocal = true;
		_localizationsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
		_currentLocalization = keyValuePair.Value;
		return keyValuePair.Value;
	}

	private IObservable<LocalizationDictionary> GetRemoteLocalization(string language)
	{
		if (_variantsProvider.TryGetLocalizationVariant(language, out var variant))
		{
			return GetLocalizationInternal(variant);
		}
		return GetLocalizationInternal(_variantsProvider.GetDefaultVariant());
	}

	private IObservable<LocalizationDictionary> GetLocalizationInternal(LocalizationVariant localizationVariant)
	{
		if (_localizationsDictionary.TryGetValue(localizationVariant.Key, out var value) && !value.IsLocal)
		{
			return Observable.Return<LocalizationDictionary>(value);
		}
		return Observable.Do<LocalizationDictionary>(_localizationLoader.Load(localizationVariant.Key), (Action<LocalizationDictionary>)delegate(LocalizationDictionary localization)
		{
			AddLocalizationToDictionary(localizationVariant.Key, localization, localizationVariant.IsDefault);
		});
	}

	private void AddLocalizationToDictionary(string language, LocalizationDictionary localization, bool isDefault)
	{
		if (!_localizationsDictionary.TryAdd(language, localization))
		{
			_localizationsDictionary[language] = localization;
		}
		if (isDefault)
		{
			_defaultLocalization = localization;
		}
	}

	private void SetCurrentLocalization(LocalizationDictionary localizationDictionary)
	{
		_currentLocalization = localizationDictionary;
		SendUpdateLocalization();
	}

	private void SendUpdateLocalization()
	{
		_onLocalizationChange?.OnNext(_currentLocalization);
	}

	public void Dispose()
	{
		_onLocalizationChange.Dispose();
		_localizationDownloadStream.Dispose();
	}
}
