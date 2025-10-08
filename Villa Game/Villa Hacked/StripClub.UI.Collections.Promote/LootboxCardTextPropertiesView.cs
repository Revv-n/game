using System;
using System.Collections.Generic;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.Model.Cards;
using UniRx;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class LootboxCardTextPropertiesView : CardView
{
	private LootboxPrefView.LootboxPrefManager prefViewManager;

	private LocalizationShortCuts localizationShortCuts;

	private LocalizationService _localizationService;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	public void Init(LootboxPrefView.LootboxPrefManager prefViewManager, LocalizationShortCuts localizationShortCuts, LocalizationService localizationService)
	{
		this.prefViewManager = prefViewManager;
		this.localizationShortCuts = localizationShortCuts;
		_localizationService = localizationService;
	}

	public override void Set(ICard card)
	{
		base.Set(card);
		prefViewManager.HideAll();
		DisplayPrefs(card);
	}

	private void DisplayPrefs(ICard card)
	{
		_disposables.Clear();
		foreach (int prefsNumber in localizationShortCuts.GetPrefsNumbers(card))
		{
			PrefView prefView = prefViewManager.GetView();
			string key = localizationShortCuts.LootboxPrefValue(card, prefsNumber);
			string key2 = localizationShortCuts.LootboxPrefName(card, prefsNumber);
			IReadOnlyReactiveProperty<string> obj = _localizationService.ObservableText(key2);
			IReadOnlyReactiveProperty<string> val = _localizationService.ObservableText(key);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe(Observable.CombineLatest((IObservable<string>)obj, (IObservable<string>)val, (string name, string value) => new { name, value }), result =>
			{
				prefView.Init(result.name, result.value);
				prefView.Display(isOn: true);
			}), (ICollection<IDisposable>)_disposables);
		}
	}
}
