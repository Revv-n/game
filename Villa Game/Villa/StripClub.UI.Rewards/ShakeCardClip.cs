using System;
using Spine;
using Spine.Unity;
using StripClub.Model.Cards;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class ShakeCardClip : Clip
{
	[Serializable]
	public struct Settings
	{
		public GameObject Explosion;

		public GameObject SkeletonContainer;

		public SkeletonAnimation Skeleton;

		public Gradient Gradient;

		public string AnimationName;
	}

	[Serializable]
	public class RarityShakeDictionary : SerializableDictionary<Rarity, Settings>
	{
	}

	[SerializeField]
	private RarityShakeDictionary raritySettings;

	[SerializeField]
	private ParticleSystem sparks;

	private Settings setting;

	private Spine.AnimationState anim;

	private bool isPlaying;

	public void Init(Rarity rarity)
	{
		setting = raritySettings[rarity];
	}

	public override void Play()
	{
		if (!isPlaying)
		{
			isPlaying = true;
			base.gameObject.SetActive(value: true);
			setting.SkeletonContainer.SetActive(value: true);
			SetEffects(setting.Gradient);
			setting.Explosion.SetActive(value: true);
			anim = StartAnimation(setting.Skeleton);
			anim.Complete += OnComplete;
		}
	}

	private void SetEffects(Gradient gradient)
	{
		sparks.gameObject.SetActive(value: true);
		ParticleSystem.MainModule main = sparks.main;
		main.startColor = gradient;
	}

	private Spine.AnimationState StartAnimation(SkeletonAnimation skeleton)
	{
		base.gameObject.SetActive(value: true);
		skeleton.ClearState();
		skeleton.AnimationState.ClearTracks();
		skeleton.AnimationState.SetAnimation(0, setting.AnimationName, loop: false);
		skeleton.Update(0.001f);
		return skeleton.AnimationState;
	}

	private void OnComplete(TrackEntry trackEntry)
	{
		Stop();
	}

	public override void Stop()
	{
		if (isPlaying)
		{
			isPlaying = false;
			anim.Complete -= OnComplete;
			Reset();
			base.gameObject.SetActive(value: false);
			base.Stop();
		}
	}

	private void Reset()
	{
		setting.Explosion?.gameObject.SetActive(value: false);
		setting.SkeletonContainer?.SetActive(value: false);
	}
}
