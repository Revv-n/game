using System;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Level.UI;

public class LevelUpWindow : Window
{
	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private Button playMergeButton;

	[SerializeField]
	private Button lvlUpButton;

	[SerializeField]
	protected Button fadeCloser;

	[SerializeField]
	protected WindowSoundSO windowSoundSO;

	[SerializeField]
	private LevelUpWindowAnimations animations;

	[SerializeField]
	private LvlUpProgressAnimation lvlUpProgressAnimation;

	[SerializeField]
	private WindowOpener openMerge;

	[SerializeField]
	private WindowOpener closeMain;

	[SerializeField]
	private TextMeshProUGUI nextLevelNumber;

	[SerializeField]
	private TextMeshProUGUI levelNumber;

	[SerializeField]
	private ProgressBarTMPro progress;

	[SerializeField]
	private TextMeshProValueStates chestTextRarity;

	[SerializeField]
	private TMProColorStates chestTextRarityColor;

	[SerializeField]
	private SpriteStates lootboxSpriteStates;

	private LevelArgsManager levelArgsManager;

	private LootboxCollection lootboxCollection;

	private PlayerExperience playerExp;

	private IPlayerExpController playerExpController;

	private ILootboxOpener lootboxOpener;

	protected IAudioPlayer audioPlayer;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (windowSoundSO == null)
		{
			Debug.LogError("Empty window sound", this);
		}
	}

	[Inject]
	private void InitLevelUp(LevelArgsManager sellArgsManager, LootboxCollection lootboxCollection, PlayerExperience playerExperience, IPlayerExpController playerExpController, ILootboxOpener lootboxOpener, IAudioPlayer audioPlayer)
	{
		levelArgsManager = sellArgsManager;
		this.lootboxCollection = lootboxCollection;
		playerExp = playerExperience;
		this.playerExpController = playerExpController;
		this.lootboxOpener = lootboxOpener;
		this.audioPlayer = audioPlayer;
	}

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		lvlUpProgressAnimation.Init();
	}

	public override void Open()
	{
		audioPlayer.PlayAudioClip2D(windowSoundSO.Open);
		base.Open();
		animations.ShowPopUp();
		UpdateValues(playerExp.XP.Value, playerExpController.Target.Value);
		fadeCloser.onClick.AddListener(Close);
		closeButton.onClick.AddListener(Close);
		playMergeButton.onClick.AddListener(PlayMerge);
	}

	public override void Close()
	{
		lvlUpButton.onClick.RemoveListener(TryLevelUp);
		closeButton.onClick.RemoveListener(Close);
		playMergeButton.onClick.RemoveListener(PlayMerge);
		fadeCloser.onClick.RemoveListener(Close);
		audioPlayer.PlayAudioClip2D(windowSoundSO.Close);
		animations.Hide().OnComplete(base.Close);
	}

	private void TryLevelUp()
	{
	}

	private void PlayMerge()
	{
		Close();
		if (!windowsManager.GetOpened().Any((IWindow _window) => _window is MergeWindow))
		{
			openMerge.OpenOnly();
		}
	}

	private void UpdateValues(int newValue, int maxValue)
	{
		levelNumber.text = playerExp.Level.Value.ToString();
		nextLevelNumber.text = (playerExp.Level.Value + 1).ToString();
		progress.Init(newValue, maxValue, 0f);
		SetRewards(levelArgsManager.GetArgs(playerExp.Level.Value));
		UpdateCompletion();
	}

	private void UpdateCompletion()
	{
		bool flag = progress.IsComplete();
		lvlUpButton.gameObject.SetActive(flag);
		playMergeButton.gameObject.SetActive(!flag);
		if (progress.IsComplete())
		{
			lvlUpProgressAnimation.Show();
		}
		else
		{
			lvlUpProgressAnimation.Hide();
		}
	}

	private void SetRewards(LevelsArgs args)
	{
		try
		{
			int rarity = (int)lootboxCollection.Get(args.ChestId).Rarity;
			chestTextRarity.Set(rarity);
			chestTextRarityColor.Set(rarity);
			lootboxSpriteStates.Set(rarity);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Cant Set LevelUp Rewards ");
		}
	}
}
