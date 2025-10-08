using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.ToolTips;
using Spine.Unity;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public class CharacterObjectConfig : BaseObjectConfig
{
	[Serializable]
	public class TooltipSettingsDictionary : SerializableDictionary<TooltipType, ToolTipSettings>
	{
	}

	[SerializeField]
	public GreenT.HornyScapes.Animations.Animation GirlShowAnimation;

	[SerializeField]
	public Vector3 GlowPosition;

	[SerializeField]
	public GirlObjectSoundSO ClickSound;

	private IBundlesLoader<int, SkeletonAnimation> skinLoader;

	[field: SerializeField]
	public int CharacterID { get; set; }

	[field: SerializeField]
	public TooltipSettingsDictionary Tooltips { get; private set; }

	[field: SerializeField]
	public Vector3 Position { get; set; } = Vector3.zero;


	[field: SerializeField]
	public Vector3 Scale { get; set; } = Vector3.one;


	[field: SerializeField]
	public int SortingOrder { get; set; } = 1;


	[field: SerializeField]
	public Vector2[] ColliderPoints { get; set; }

	[field: SerializeField]
	public AnimationSettingsDictionary AnimationSettingsDictionary { get; private set; } = new AnimationSettingsDictionary();


	[Inject]
	public void Init(IBundlesLoader<int, SkeletonAnimation> skinLoader)
	{
		this.skinLoader = skinLoader;
	}

	public IObservable<CharacterAnimationSettings> GetSkinSettings(int skinID)
	{
		if (AnimationSettingsDictionary.TryGetValue(skinID, out var settings))
		{
			return Observable.Return<CharacterAnimationSettings>(settings);
		}
		if (settings == null)
		{
			throw new ArgumentOutOfRangeException($"House. Object of character (id: {CharacterID}) doesn't have skin with ID: {skinID}");
		}
		return Observable.Select<SkeletonAnimation, CharacterAnimationSettings>(skinLoader.Load(skinID), (Func<SkeletonAnimation, CharacterAnimationSettings>)((SkeletonAnimation _) => settings));
	}
}
