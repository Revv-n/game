using System;
using GreenT.HornyScapes.Presents.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Presents.Cheats;

public class PresentsCheats : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _presentButton;

	[SerializeField]
	private Image _presentIcon;

	private PresentsManager _presentsManager;

	private readonly string _presentIdFormat = "present_{0}";

	[Inject]
	public void Init(PresentsManager presentsManager)
	{
		_presentsManager = presentsManager;
	}

	private void Start()
	{
		_presentButton.onClick.AddListener(AddPresents);
		OnEnterValue(_inputField.text);
		_inputField.onValueChanged.AddListener(OnEnterValue);
	}

	private void AddPresents()
	{
		string text = _inputField.text;
		try
		{
			string[] array = text.Split(":", StringSplitOptions.None);
			string id = string.Format(arg0: array[0], format: _presentIdFormat);
			int value = int.Parse(array[1]);
			_presentsManager.Get(id)?.AddCount(value);
		}
		catch (Exception innerException)
		{
			innerException.SendException("Can't parse input string: " + text + ". Input should be: ID:Amount");
		}
	}

	protected void OnEnterValue(string value)
	{
		string text = string.Format(_presentIdFormat, value);
		if (_presentsManager.TryGet(text, out var present))
		{
			_presentIcon.sprite = present?.Icon;
			_presentButton.interactable = true;
		}
		else if (TryParseArray(text, out present))
		{
			_presentIcon.sprite = present?.Icon;
			_presentButton.interactable = true;
		}
		else
		{
			_presentIcon.sprite = null;
			_presentButton.interactable = false;
		}
	}

	private bool TryParseArray(string value, out Present present)
	{
		string[] array = value.Split(':', StringSplitOptions.None);
		present = null;
		if (array.Length == 2)
		{
			return _presentsManager.TryGet(array[0], out present);
		}
		return false;
	}

	private void OnDestroy()
	{
		_presentButton.onClick.RemoveListener(AddPresents);
		_inputField.onValueChanged.RemoveListener(OnEnterValue);
	}
}
