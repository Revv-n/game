using System;
using System.Collections.Generic;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using Merge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatPocket : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button pocketBtn;

	[SerializeField]
	private Image pocketBtnImage;

	public string inputedSet = "Tree6, Bricks6";

	private GIConfig config;

	private List<GIConfig> configs = new List<GIConfig>();

	private IMergeIconProvider _iconProvider;

	private GameItemConfigManager _gameItemConfigManager;

	[SerializeField]
	private InputSettingCheats inputSetting;

	[Inject]
	public void Init(IMergeIconProvider iconProvider, GameItemConfigManager gameItemConfigManager)
	{
		_iconProvider = iconProvider;
		_gameItemConfigManager = gameItemConfigManager;
	}

	protected void Awake()
	{
		OnEnterValue(inputField.text);
		inputField.onValueChanged.AddListener(OnEnterValue);
		pocketBtn.onClick.AddListener(AddItem);
	}

	private void Update()
	{
		if (inputSetting.DeleteAllItemsInPocket.IsPressedKeys)
		{
			Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.ClearDataInPocket();
		}
	}

	[EditorButton]
	public void AddSet()
	{
		string[] array = inputedSet.Split(',', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		string[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			GIKey giKey = GIKey.Parse(array2[j]);
			AddItem(giKey);
		}
	}

	private void AddItem(GIKey giKey)
	{
		Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.AddItemToQueue(giKey);
	}

	private void AddItem()
	{
		if (configs.Count != 0)
		{
			foreach (GIConfig config in configs)
			{
				AddItem(config.Key);
			}
			return;
		}
		AddItem(this.config.Key);
	}

	protected void OnEnterValue(string value)
	{
		if (int.TryParse(value, out var result) && _gameItemConfigManager.TryGetConfig(result, out config))
		{
			pocketBtnImage.sprite = _iconProvider.GetSprite(config.Key);
			pocketBtn.interactable = true;
		}
		else if (TryParseArray(value))
		{
			pocketBtnImage.sprite = _iconProvider.GetSprite(configs[configs.Count - 1].Key);
			pocketBtn.interactable = true;
		}
		else
		{
			pocketBtn.interactable = false;
			pocketBtnImage.sprite = null;
		}
	}

	private bool TryParseArray(string value)
	{
		configs.Clear();
		string[] array = value.Split(':', '-');
		if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && result2 > result && result > 0)
		{
			for (int i = result; i <= result2; i++)
			{
				if (_gameItemConfigManager.TryGetConfig(i, out config))
				{
					configs.Add(config);
				}
			}
			return true;
		}
		return false;
	}
}
