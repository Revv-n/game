using System;
using GreenT.HornyScapes.Lootboxes;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class LevelDropView : MonoView
{
	public class Manager : ViewManager<LevelDropView>
	{
	}

	[SerializeField]
	private Image _icon;

	private GameSettings _gameSettings;

	private IDisposable _iconChangeStream;

	[Inject]
	private void Construct(GameSettings gameSettings)
	{
		_gameSettings = gameSettings;
	}

	public void Set()
	{
		if (!_gameSettings.RewPlaceholder.TryGetValue(RewType.Level, out var value))
		{
			Debug.LogException(new ArgumentOutOfRangeException($"Dictionary [{_gameSettings.RewPlaceholder.GetType()} missing type {RewType.Level}]"));
		}
		else
		{
			_icon.sprite = value.LevelRewardBaseIcon;
		}
	}
}
