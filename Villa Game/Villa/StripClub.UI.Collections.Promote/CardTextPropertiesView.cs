using System;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.Model.Cards;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class CardTextPropertiesView : CharacterView
{
	[SerializeField]
	private TextMeshProUGUI description;

	private PrefView.Manager prefViewManager;

	private LocalizationShortCuts localizationShortCuts;

	private LocalizationService _localizationService;

	private IDisposable _descriptionDisposable;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	public void Init(PrefView.Manager prefViewManager, LocalizationShortCuts localizationShortCuts, LocalizationService localizationService)
	{
		this.prefViewManager = prefViewManager;
		this.localizationShortCuts = localizationShortCuts;
		_localizationService = localizationService;
	}

	public override void Set(CharacterSettings card)
	{
		base.Set(card);
		_descriptionDisposable?.Dispose();
		prefViewManager.HideAll();
		DisplayPrefs(card.Public);
		_descriptionDisposable = _localizationService.ObservableText(card.Public.DescriptionKey).Subscribe(delegate(string descr)
		{
			description.text = descr;
		});
	}

	private void DisplayPrefs(ICard card)
	{
		_disposables.Clear();
		foreach (int prefsNumber in localizationShortCuts.GetPrefsNumbers(card))
		{
			PrefView prefView = prefViewManager.GetView();
			string key = localizationShortCuts.CardParameterValueKey(card, prefsNumber);
			string key2 = localizationShortCuts.CardParameterNameKey(card, prefsNumber);
			IReadOnlyReactiveProperty<string> left = _localizationService.ObservableText(key2);
			IReadOnlyReactiveProperty<string> right = _localizationService.ObservableText(key);
			left.CombineLatest(right, (string name, string value) => new { name, value }).Subscribe(result =>
			{
				prefView.Init(result.name, result.value);
				prefView.Display(isOn: true);
			}).AddTo(_disposables);
		}
	}

	private void OnDisable()
	{
		_disposables?.Clear();
	}

	private void OnDestroy()
	{
		_disposables?.Dispose();
	}
}
