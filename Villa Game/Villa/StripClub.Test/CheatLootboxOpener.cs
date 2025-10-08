using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using Spine.Unity;
using StripClub.Model.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Test;

public class CheatLootboxOpener : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private protected Button openButton;

	private LootboxCollection lootboxCollection;

	private LootboxOpener rewardController;

	private IBundlesLoader<int, SkeletonAnimation> characterSkinLoader;

	protected Lootbox lootbox;

	[Inject]
	private void Construct(LootboxCollection lootboxCollection, LootboxOpener rewardController, IBundlesLoader<int, SkeletonAnimation> characterSkinLoader)
	{
		this.lootboxCollection = lootboxCollection;
		this.rewardController = rewardController;
		this.characterSkinLoader = characterSkinLoader;
	}

	private void Awake()
	{
		OnEnterValue(inputField.text);
		inputField.onValueChanged.AddListener(OnEnterValue);
		openButton.onClick.AddListener(delegate
		{
			OpenLootbox(lootbox);
		});
	}

	public void OpenLootbox(Lootbox lootbox)
	{
		if (lootbox != null)
		{
			if (!lootbox.CharacterIdPossibleDrops.Any())
			{
				rewardController.Open(lootbox, CurrencyAmplitudeAnalytic.SourceType.None);
			}
			(from id in lootbox.CharacterIdPossibleDrops.ToObservable()
				select characterSkinLoader.Load(id)).ToList().Subscribe(delegate
			{
				rewardController.Open(lootbox, CurrencyAmplitudeAnalytic.SourceType.None);
			});
		}
	}

	protected void OnEnterValue(string value)
	{
		if (int.TryParse(value, out var targetID) && lootboxCollection.Collection.Any((Lootbox _lootbox) => _lootbox.ID == targetID))
		{
			lootbox = lootboxCollection.Collection.First((Lootbox _lootbox) => _lootbox.ID == targetID);
			openButton.interactable = true;
		}
		else
		{
			lootbox = null;
			openButton.interactable = false;
		}
	}

	private void OnDestroy()
	{
		inputField.onValueChanged.RemoveListener(OnEnterValue);
		openButton.onClick.RemoveAllListeners();
	}
}
