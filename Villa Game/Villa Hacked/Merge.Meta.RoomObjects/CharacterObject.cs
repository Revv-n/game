using System;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Meta.Girls;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.ToolTips;
using GreenT.Utilities;
using Spine;
using Spine.Unity;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace Merge.Meta.RoomObjects;

public class CharacterObject : GameRoomObject<CharacterObjectConfig>
{
	[SerializeField]
	private GirlToolTipController controller;

	[SerializeField]
	private PolygonCollider2D collider;

	[SerializeField]
	private CollideTracker collideTracker;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation animation;

	private IAudioPlayer audioPlayer;

	private CharacterManagerState charactersState;

	private SkinManager skinManager;

	private IBundlesLoader<int, SkeletonAnimation> skinLoader;

	private GirlClickBooster clickBooster;

	private SkeletonAnimation spineAnimation;

	private MeshRenderer animationMeshRenderer;

	private int characterID;

	private int skinSettingsId;

	private bool bundlesLoaded;

	private IDisposable clickStream;

	private IObservable<(CharacterAnimationSettings, SkeletonAnimation)> animationLoadObservable;

	private IDisposable animationLoadStream;

	private int SkinVisualId
	{
		get
		{
			if (skinSettingsId != 0)
			{
				return skinSettingsId;
			}
			return characterID;
		}
	}

	public bool IsSetAsVisible { get; private set; } = true;


	public Vector3 Position { get; private set; }

	public Vector3 Extents { get; private set; }

	[Inject]
	public void Construct(IAudioPlayer audioPlayer, CharacterManagerState charactersState, SkinManager skinManager, IBundlesLoader<int, SkeletonAnimation> skinLoader, DiContainer container)
	{
		this.audioPlayer = audioPlayer;
		this.charactersState = charactersState;
		this.skinManager = skinManager;
		this.skinLoader = skinLoader;
		clickBooster = container.InstantiateComponent<GirlClickBooster>(ViewRoot.gameObject);
	}

	public override void Init(RoomStateData data, CharacterObjectConfig config)
	{
		SetConfig(config);
		characterID = config.CharacterID;
		if (config.Tooltips == null)
		{
			Debug.LogError(config.name + " Empties girl tool tip settings");
		}
		controller.Init(config.Tooltips);
		int selectedSkin = data.SelectedSkin;
		bool num = selectedSkin == 0;
		int num2 = (num ? characterID : selectedSkin);
		if (num ? IsCharacterAvailable() : IsSkinAvailable(num2))
		{
			IsSetAsVisible = true;
			DownloadAndSetSkin(num2, selectedSkin);
		}
		SetupAppearanceAnimation(config);
		base.Init(data, config);
	}

	public void ReDownloadAndShow()
	{
		if (!IsSetAsVisible)
		{
			IsSetAsVisible = true;
			DownloadAndSetSkin(SkinVisualId, skinSettingsId);
		}
	}

	public void HideAndUnload()
	{
		if (animationLoadObservable != null || !bundlesLoaded)
		{
			return;
		}
		if (animationLoadObservable != null)
		{
			animationLoadStream.Dispose();
		}
		IsSetAsVisible = false;
		SetVisible(visible: false);
		skinLoader.ReleaseBundle(characterID);
		bundlesLoaded = false;
		foreach (GreenT.HornyScapes.Characters.Skins.Skin item in skinManager.GetSkinByCharacter(characterID))
		{
			if (!item.IsDataEmpty)
			{
				skinLoader.ReleaseBundle(item.ID);
			}
		}
	}

	public override IObservable<Bounds> GetBounds()
	{
		if (!animationMeshRenderer)
		{
			return Observable.Select<(CharacterAnimationSettings, SkeletonAnimation), Bounds>(DownloadAndSetSkin(characterID, 0), (Func<(CharacterAnimationSettings, SkeletonAnimation), Bounds>)(((CharacterAnimationSettings, SkeletonAnimation) _) => animationMeshRenderer.bounds));
		}
		return Observable.Return<Bounds>(animationMeshRenderer.bounds);
	}

	public override void Highlight(HighlightType highlightType)
	{
		throw new NotImplementedException();
	}

	public override void SetSkin(int skinID)
	{
		if (skinSettingsId != skinID)
		{
			skinSettingsId = skinID;
			animationLoadStream?.Dispose();
			DownloadAndSetSkin(SkinVisualId, skinSettingsId);
		}
	}

	private IObservable<(CharacterAnimationSettings, SkeletonAnimation)> DownloadAndSetSkin(int skinVisualId, int skinSettingsId)
	{
		if (animationLoadObservable != null)
		{
			return animationLoadObservable;
		}
		animationLoadObservable = Observable.Share<(CharacterAnimationSettings, SkeletonAnimation)>(Observable.Finally<(CharacterAnimationSettings, SkeletonAnimation)>(Observable.Do<(CharacterAnimationSettings, SkeletonAnimation)>(Observable.SelectMany<CharacterAnimationSettings, (CharacterAnimationSettings, SkeletonAnimation)>(base.Config.GetSkinSettings(skinSettingsId), (Func<CharacterAnimationSettings, IObservable<(CharacterAnimationSettings, SkeletonAnimation)>>)((CharacterAnimationSettings skin) => Observable.Select<SkeletonAnimation, (CharacterAnimationSettings, SkeletonAnimation)>(skinLoader.Load(skinVisualId), (Func<SkeletonAnimation, (CharacterAnimationSettings, SkeletonAnimation)>)((SkeletonAnimation animation) => (skin: skin, animation: animation))))), (Action<(CharacterAnimationSettings, SkeletonAnimation)>)delegate((CharacterAnimationSettings skin, SkeletonAnimation animation) pair)
		{
			DisplaySkin(pair.skin, pair.animation);
			SetView(this.skinSettingsId);
			SetVisible(IsVisible);
			bundlesLoaded = true;
			Position = animationMeshRenderer.bounds.center;
			Extents = animationMeshRenderer.bounds.extents;
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (Action)delegate
		{
			animationLoadObservable = null;
		}));
		animationLoadStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CharacterAnimationSettings, SkeletonAnimation)>(animationLoadObservable), (Component)this);
		return animationLoadObservable;
	}

	public override void SetView(int viewNumber)
	{
		if (!spineAnimation)
		{
			return;
		}
		try
		{
			Spine.Skin skin = spineAnimation.Skeleton.Data.Skins.Items[0];
			spineAnimation.Skeleton.SetSkin(skin.Name);
			base.Data.SelectedSkin = viewNumber;
		}
		catch (Exception innerException)
		{
			innerException.SendException(this?.ToString() + " exception on applying skin");
		}
	}

	public override void SetVisible(bool visible)
	{
		if (!spineAnimation)
		{
			return;
		}
		spineAnimation.gameObject.SetActive(visible);
		collider.enabled = visible;
		clickStream?.Dispose();
		if (!visible)
		{
			return;
		}
		SoundOnClick();
		controller.ShowOpener.ReadyToActivate.Value = true;
		foreach (Material item in animationMeshRenderer.sharedMaterials.Where((Material _mat) => _mat.HasProperty("_Color")))
		{
			Color color = item.color;
			color.a = 0f;
			item.color = color;
		}
		animation.Play();
		void SoundOnClick()
		{
			if (!(base.Config.ClickSound == null))
			{
				clickStream = ObservableExtensions.Subscribe<Unit>(collideTracker.OnClick, (Action<Unit>)delegate
				{
					OnClick();
				});
			}
		}
	}

	private void SetupAppearanceAnimation(CharacterObjectConfig config)
	{
		if (config.GirlShowAnimation == null)
		{
			return;
		}
		GreenT.HornyScapes.Animations.Animation animation = UnityEngine.Object.Instantiate(config.GirlShowAnimation, base.transform);
		if (!(this.animation != null))
		{
			ChainAnimationGroup chainAnimationGroup = base.gameObject.AddComponent<ChainAnimationGroup>();
			SpineObjectAnimation[] components = animation.GetComponents<SpineObjectAnimation>();
			foreach (SpineObjectAnimation spineObjectAnimation in components)
			{
				ShaderFinder.UpdateMaterial(spineObjectAnimation.GlowRenderer.sharedMaterial);
				spineObjectAnimation.SpineRenderer = animationMeshRenderer;
				spineObjectAnimation.GlowRenderer.transform.localPosition = config.GlowPosition;
				chainAnimationGroup.Animations.Add(spineObjectAnimation);
			}
			this.animation = chainAnimationGroup;
		}
	}

	private void DisplaySkin(CharacterAnimationSettings settings, SkeletonAnimation skeletonAnimation)
	{
		if ((bool)spineAnimation)
		{
			UnityEngine.Object.Destroy(spineAnimation.gameObject);
		}
		spineAnimation = UnityEngine.Object.Instantiate(skeletonAnimation, ViewRoot);
		spineAnimation.transform.localPosition = settings.Position;
		spineAnimation.transform.localScale = settings.Scale;
		animationMeshRenderer = spineAnimation.GetComponent<MeshRenderer>();
		ShaderFinder.UpdateMaterial(animationMeshRenderer.sharedMaterial);
		Material[] sharedMaterials = animationMeshRenderer.sharedMaterials;
		foreach (Material obj in sharedMaterials)
		{
			ShaderFinder.UpdateMaterial(obj);
			obj.color = Color.white;
		}
		UpdateSpineAnimation(animationMeshRenderer);
		collider.points = settings.ColliderPoints;
		clickBooster.Init(spineAnimation);
		controller.Init(settings.ToolTipSettings);
	}

	private void UpdateSpineAnimation(MeshRenderer animationMeshRenderer)
	{
		SpineObjectAnimation[] componentsInChildren = base.transform.GetComponentsInChildren<SpineObjectAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SpineRenderer = animationMeshRenderer;
		}
	}

	private void OnClick()
	{
		audioPlayer?.PlayFadingAudioClip2D(base.Config.ClickSound.Sound);
	}

	public bool IsCharacterAvailable()
	{
		foreach (int unlockedCharacterID in charactersState.UnlockedCharacterIDs)
		{
			if (unlockedCharacterID == characterID)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsSkinAvailable(int id)
	{
		foreach (GreenT.HornyScapes.Characters.Skins.Skin item in skinManager.Collection)
		{
			if (item.ID == id && item.IsOwned && item.Locker.IsOpen.Value && item.IsDataEmpty)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		clickStream?.Dispose();
		animationLoadStream?.Dispose();
	}

	public override string ToString()
	{
		return base.ToString() + " " + base.gameObject.name;
	}
}
