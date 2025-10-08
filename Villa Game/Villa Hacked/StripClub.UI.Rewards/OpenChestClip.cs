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
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		settings = raritySettings[rarity];
		chestIcon.sprite = settings.ClosedChest;
		sunlight.color = settings.SunlightColor;
		glow.color = settings.GlowColor;
		colorExplode.startColor = settings.ParticleColor;
		TrailModule trails = colorExplode.trails;
		((TrailModule)(ref trails)).colorOverLifetime = MinMaxGradient.op_Implicit(settings.ColorOverLifeTrail);
		((TrailModule)(ref trails)).colorOverTrail = MinMaxGradient.op_Implicit(settings.ColorOverTrail);
	}

	public override void Play()
	{
		base.gameObject.SetActive(value: true);
		audioPlayer?.PlayAudioClip2D(fallingChestSound.Sound);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Timer(TimeSpan.FromSeconds(changeSpriteToOpenedDelay)), (Component)this), (Action<long>)delegate
		{
			chestIcon.sprite = settings.OpenedChest;
		}), (Component)this);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Timer(TimeSpan.FromSeconds(endAnimation)), (Component)this), (Action<long>)delegate
		{
			base.gameObject.SetActive(value: false);
			Stop();
		}), (Component)this);
	}
}
