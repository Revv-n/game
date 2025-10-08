using System;
using GreenT.Data;
using Merge;
using UniRx;
using UnityEngine;

namespace GreenT.Localizations;

[MementoHolder]
public class LocalizationState : ISavableState
{
	[Serializable]
	public class LocalizationMemento : Memento
	{
		public string CurrentLanguage { get; }

		public LocalizationMemento(LocalizationState localization)
			: base(localization)
		{
			CurrentLanguage = localization.CurrentLanguage;
		}
	}

	private const string SaveKey = "loc.save.key";

	private readonly Subject<string> _onLanguageChange = new Subject<string>();

	public string CurrentLanguage { get; private set; }

	public IObservable<string> OnLanguageChange => _onLanguageChange.AsObservable();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public LocalizationState(ISaver saver)
	{
		saver.Add(this);
	}

	public void UpdateLanguage(string language)
	{
		CurrentLanguage = language;
		_onLanguageChange.OnNext(language);
	}

	public void UpdateLocalLanguage(string language)
	{
		PlayerPrefs.SetString("loc.save.key", language);
	}

	public bool TryGetLocalLanguage(out string language)
	{
		language = PlayerPrefs.GetString("loc.save.key", string.Empty);
		return !language.IsNullOrEmpty();
	}

	public void Initialize()
	{
		_onLanguageChange.OnNext(CurrentLanguage);
	}

	public string UniqueKey()
	{
		return "loc.save.key";
	}

	public Memento SaveState()
	{
		return new LocalizationMemento(this);
	}

	public void LoadState(Memento memento)
	{
		LocalizationMemento localizationMemento = (LocalizationMemento)memento;
		CurrentLanguage = localizationMemento.CurrentLanguage;
	}
}
