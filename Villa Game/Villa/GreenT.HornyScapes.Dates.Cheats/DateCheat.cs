using System;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Dates.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Dates.Cheats;

public class DateCheat : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _startButton;

	private DateProvider _dateProvider;

	private DateController _dateController;

	[Inject]
	private void Init(DateProvider dateProvider, DateController dateController)
	{
		_dateProvider = dateProvider;
		_dateController = dateController;
	}

	private void Start()
	{
		_startButton.onClick.AddListener(StartDate);
	}

	private void StartDate()
	{
		if (int.TryParse(_inputField.text, out var result))
		{
			Date date = _dateProvider.Get(result);
			if (date == null)
			{
				throw new NullReferenceException($"No date with such id: {result}").LogException();
			}
			_dateController.StartDate(date);
		}
	}

	private void OnDestroy()
	{
		_startButton.onClick.RemoveListener(StartDate);
	}
}
