using System;
using GreenT.HornyScapes.Sounds;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class OpenChestClip : Clip
{
	[Serializable]
	public struct Settings
	{
		public Sprite OpenedChest;

		public Sprite ClosedChest;

		public Color SunlightColor;

		public Color GlowColor;

		public Color ParticleColor;

		public Gradient ColorOverLifeTrail;

		public Gradient ColorOverTrail;
	}

	[Serializable]
	public class RaritySpriteDictionary : SerializableDictionary<Rarity, Settings>
	{
	}

	[Inject]
	private IAudioPlayer audioPlayer;

	[SerializeField]
	private Image chestIcon;

	[SerializeField]
	private Image glow;

	[SerializeField]
	private Image sunlight;

	[SerializeField]
	private ParticleSystem colorExplode;

	[SerializeField]
	private RaritySpriteDictionary raritySettings;

	[SerializeField]
	private SoundSO fallingChestSound;

	[Header("Animation settings")]
	[SerializeField]
	private float changeSpriteToOpenedDelay = 1f;

	[SerializeField]
	private float endAnimation = 2f;

	private Settings settings;

	private void OnValidate()
	{
		if (fallingChestSound == null)
		{
			Debug.LogError("Lootboxes: OpenChest has empty audio");
		}
	}

	[Obsolete]
	public void Init(Rarity rarity)
	{
		settings = raritySettings[rarity];
		chestIcon.sprite = settings.ClosedChest;
		sunlight.color = settings.SunlightColor;
		glow.color = settings.GlowColor;
		colorExplode.startColor = settings.ParticleColor;
		ParticleSystem.TrailModule trails = colorExplode.trails;
		trails.colorOverLifetime = settings.ColorOverLifeTrail;
		trails.colorOverTrail = settings.ColorOverTrail;
	}

	public override void Play()
	{
		base.gameObject.SetActive(value: true);
		audioPlayer?.PlayAudioClip2D(fallingChestSound.Sound);
		Observable.Timer(TimeSpan.FromSeconds(changeSpriteToOpenedDelay)).TakeUntilDisable(this).Subscribe(delegate
		{
			chestIcon.sprite = settings.OpenedChest;
		})
			.AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(endAnimation)).TakeUntilDisable(this).Subscribe(delegate
		{
			base.gameObject.SetActive(value: false);
			Stop();
		})
			.AddTo(this);
	}
}
