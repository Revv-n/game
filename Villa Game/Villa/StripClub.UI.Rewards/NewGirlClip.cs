using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.Sounds;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Model.Cards;
using StripClub.UI.Collections.Promote;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class NewGirlClip : Clip
{
	[Serializable]
	public struct Settings
	{
		public Color RarityColor;

		public Sprite RightFrame;

		public Sprite LeftFrame;

		public Color EffectColor;

		public ParticleSystem[] Effects;

		public GameObject TextFx;

		public Color FadeColor;

		public Image[] Fades;

		public Material Material;
	}

	[Serializable]
	public class RarityColorsDictionary : SerializableDictionary<Rarity, Settings>
	{
	}

	private const string TEXT_BACKGROUND_EFFECT_COLOR = "_TintColor";

	private const string ANIMATOR_STATE_NAME = "IsClicked";

	private const string DELAY_KEY = "newgirl_tap_delay";

	private readonly int animatorState = Animator.StringToHash("IsClicked");

	[SerializeField]
	private AfterShowNewGirlClip afterExplode;

	[SerializeField]
	private float afterExplodeDelay = 1f;

	[SerializeField]
	private TextMeshProUGUI characterName;

	[SerializeField]
	private TextMeshProUGUI characterRarity;

	[SerializeField]
	private Image rightFrame;

	[SerializeField]
	private Image rightFrameLong;

	[SerializeField]
	private Image leftFrame;

	[SerializeField]
	private Image TextBackgroundEffect;

	[SerializeField]
	private RarityColorsDictionary colorSettings;

	[SerializeField]
	private GameObject skipOneButton;

	[SerializeField]
	private GameObject skipAllButton;

	[SerializeField]
	private Image newGirlFullScreen;

	[SerializeField]
	private SoundSO newGirlSound;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private LootboxCardTextPropertiesView info;

	[SerializeField]
	private GameObject newGirlText;

	[SerializeField]
	private NewGirlBonusView bonusView;

	private IDisposable _inputDispose;

	private Settings settings;

	private IAudioPlayer audioPlayer;

	private IWindowsManager uiManager;

	private IConstants<float> floatConstants;

	private LocalizationService _localizationService;

	private IDisposable _nameDisposable;

	private UIClickSuppressor _clickSuppressor;

	private CharacterSettingsManager characterSettingsManager;

	private SkinManager skinManager;

	[Inject]
	public void Init(IAudioPlayer audioPlayer, IWindowsManager uiManager, IConstants<float> floatConstants, LocalizationService localizationService, UIClickSuppressor uIClickSuppressor, CharacterSettingsManager characterSettingsManager, SkinManager skinManager)
	{
		this.audioPlayer = audioPlayer;
		this.uiManager = uiManager;
		this.floatConstants = floatConstants;
		_localizationService = localizationService;
		_clickSuppressor = uIClickSuppressor;
		this.characterSettingsManager = characterSettingsManager;
		this.skinManager = skinManager;
	}

	private void OnValidate()
	{
		if (newGirlSound == null)
		{
			Debug.LogError(GetType().Name + ": NewGirlClip has empty audio");
		}
	}

	public void Init(ICharacter character, bool isGirlNew = true)
	{
		CharacterSettings characterSettings = characterSettingsManager.Get(character.ID);
		string nameKey;
		Sprite splashArt;
		if (characterSettings != null && characterSettings.SkinID != 0)
		{
			Skin skin = skinManager.Get(characterSettings.SkinID);
			nameKey = skin.NameKey;
			splashArt = skin.Data.SplashArt;
		}
		else
		{
			nameKey = character.NameKey;
			splashArt = character.GetBundleData().SplashArt;
		}
		settings = colorSettings[character.Rarity];
		_nameDisposable?.Dispose();
		_nameDisposable = _localizationService.ObservableText(nameKey).Subscribe(delegate(string text)
		{
			characterName.text = text;
		});
		characterRarity.text = character.Rarity.ToString();
		rightFrame.sprite = settings.RightFrame;
		rightFrameLong.sprite = settings.RightFrame;
		leftFrame.sprite = settings.LeftFrame;
		characterRarity.color = settings.RarityColor;
		characterRarity.fontMaterial = settings.Material;
		newGirlFullScreen.sprite = splashArt;
		settings.TextFx.SetActive(value: true);
		TextBackgroundEffect.material.SetColor("_TintColor", settings.EffectColor);
		ParticleSystem[] effects = settings.Effects;
		foreach (ParticleSystem obj in effects)
		{
			obj.gameObject.SetActive(value: true);
			ParticleSystem.MainModule main = obj.main;
			main.startColor = settings.EffectColor;
		}
		Image[] fades = settings.Fades;
		foreach (Image obj2 in fades)
		{
			obj2.gameObject.SetActive(value: true);
			obj2.color = settings.FadeColor;
		}
		info.Set(character);
		newGirlText.SetActive(isGirlNew);
		bonusView.Init(character.Bonus as CharacterMultiplierBonus);
	}

	public override void Play()
	{
		uiManager.MoveWindowBackwards(uiManager.Get<ResourcesWindow>(), uiManager.Get<RewardsWindow>());
		skipOneButton.SetActive(value: false);
		skipAllButton.SetActive(value: false);
		base.gameObject.SetActive(value: true);
		audioPlayer?.PlayAudioClip2D(newGirlSound.Sound);
		_inputDispose?.Dispose();
		Observable.Timer(TimeSpan.FromSeconds(afterExplodeDelay)).TakeUntilDisable(this).Subscribe(delegate
		{
			afterExplode.Play();
		})
			.AddTo(this);
		_inputDispose = (from _ in (from _ in Observable.EveryUpdate().Delay(TimeSpan.FromSeconds(floatConstants["newgirl_tap_delay"]))
				where Input.GetMouseButtonDown(0)
				select _).DelayFrame(_clickSuppressor.Delay)
			where !_clickSuppressor.IsSuppressed.Value
			select _).Take(1).TakeUntilDisable(this).Subscribe(delegate
		{
			animator.SetTrigger(animatorState);
		})
			.AddTo(this);
	}

	public void Hide()
	{
		uiManager.MoveWindowForward(uiManager.Get<ResourcesWindow>());
		_inputDispose?.Dispose();
		_nameDisposable?.Dispose();
		base.gameObject.SetActive(value: false);
		settings.TextFx.SetActive(value: false);
		Stop();
		ParticleSystem[] effects = settings.Effects;
		for (int i = 0; i < effects.Length; i++)
		{
			effects[i].gameObject.SetActive(value: false);
		}
		Image[] fades = settings.Fades;
		for (int i = 0; i < fades.Length; i++)
		{
			fades[i].gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		skipOneButton.SetActive(value: true);
		skipAllButton.SetActive(value: true);
	}
}
