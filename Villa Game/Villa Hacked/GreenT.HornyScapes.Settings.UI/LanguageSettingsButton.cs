using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Settings.UI;

public class LanguageSettingsButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private TMP_Text _text;

	[SerializeField]
	private Sprite _select;

	[SerializeField]
	private Sprite _deactivate;

	public IObservable<LanguageSettingsButton> OnClick;

	public string Key { get; private set; }

	public void Initialization(string name, string key)
	{
		_text.text = name;
		Key = key;
		OnClick = Observable.Select<Unit, LanguageSettingsButton>(UnityEventExtensions.AsObservable((UnityEvent)_button.onClick), (Func<Unit, LanguageSettingsButton>)((Unit x) => this));
	}

	public void Select()
	{
		_button.image.sprite = _select;
	}

	public void Deselect()
	{
		_button.image.sprite = _deactivate;
	}
}
