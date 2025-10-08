using System;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class TextMeshProValueStates : StatableComponent
{
	public bool localize;

	[SerializeField]
	private IntStringDictionary statesStringDict;

	[SerializeField]
	private TextMeshProUGUI text;

	private int? current;

	private IDisposable localizationStream;

	private LocalizationService _localizationService;

	public TextMeshProUGUI Text => text;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(int stateNumber)
	{
		current = stateNumber;
		string value = statesStringDict[stateNumber];
		if (localize)
		{
			LocalizationStream(value);
		}
	}

	public void ForceSetElement(int index, string value)
	{
		statesStringDict[index] = value;
	}

	private void SetText(string value)
	{
		text.text = value;
	}

	private void LocalizationStream(string value)
	{
		localizationStream?.Dispose();
		localizationStream = _localizationService.ObservableText(value).TakeUntilDisable(this).Subscribe(SetText)
			.AddTo(this);
	}

	private void OnEnable()
	{
		if (current.HasValue && localize)
		{
			string value = statesStringDict[current.Value];
			LocalizationStream(value);
		}
	}

	private void OnValidate()
	{
		if (text == null)
		{
			text = GetComponent<TextMeshProUGUI>();
		}
	}
}
