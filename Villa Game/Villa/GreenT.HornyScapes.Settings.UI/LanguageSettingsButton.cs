using System;
using TMPro;
using UniRx;
using UnityEngine;
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
		OnClick = from x in _button.onClick.AsObservable()
			select this;
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
