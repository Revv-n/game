using System;
using GreenT.HornyScapes.GameItems;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Tasks;

public class TasksWindowRewardItem : MonoBehaviour, IPoolReturner
{
	[SerializeField]
	private Text nameLabel;

	[SerializeField]
	private Image icon;

	private GameItemConfigManager _gameItemConfigManager;

	public Action ReturnInPool { get; set; }

	public GIData Value { get; private set; }

	public void Init(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
	}

	public TasksWindowRewardItem SetReward(GIData data)
	{
		Value = data;
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(data.Key);
		nameLabel.text = configOrNull.GameItemName;
		icon.sprite = IconProvider.GetGISprite(data.Key);
		return this;
	}
}
