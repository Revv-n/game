using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipLevelView : MonoView<int>
{
	private const string LevelLocalizationKey = "ui.datestatus.{0}";

	[SerializeField]
	private TMP_Text _levelField;

	[SerializeField]
	private IntColorDictionary _colors;

	private LocalizationService _localizationService;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	private void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(int source)
	{
		base.Set(source);
		_levelField.color = _colors[source];
		_localizationDisposables?.Clear();
		string key = $"ui.datestatus.{source}";
		_localizationService.ObservableText(key).Subscribe(delegate(string text)
		{
			_levelField.text = text;
		}).AddTo(_localizationDisposables);
	}

	private void OnDestroy()
	{
		_localizationDisposables?.Dispose();
	}
}
