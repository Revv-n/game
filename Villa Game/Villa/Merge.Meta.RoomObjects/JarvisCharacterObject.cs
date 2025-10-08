using System;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Utilities;
using Spine;
using Spine.Unity;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class JarvisCharacterObject : BaseRoomObject<CharacterObjectConfig>
{
	[SerializeField]
	private PolygonCollider2D collider;

	private MeshRenderer animationMeshRenderer;

	private ILoader<int, SkeletonAnimation> characterSkinLoader;

	private IDisposable clickStream;

	private IDisposable skinLoadStream;

	private IDisposable animationLoadStream;

	private int skinID;

	[field: SerializeField]
	public JarvisGirlToolTipView Tooltip { get; private set; }

	public PolygonCollider2D Collider => collider;

	public SkeletonAnimation ObjectAnimation { get; private set; }

	public override bool IsVisible => base.gameObject.activeSelf;

	public void Inject(ILoader<int, SkeletonAnimation> characterSkinLoader)
	{
		this.characterSkinLoader = characterSkinLoader;
	}

	public override void Init(CharacterObjectConfig config)
	{
		base.Init(config);
		base.gameObject.name = $"Character ({config.ObjectName})";
		if (config.AnimationSettingsDictionary.TryGetValue(0, out var defaultSkin))
		{
			animationLoadStream?.Dispose();
			animationLoadStream = characterSkinLoader.Load(config.CharacterID).Subscribe(delegate(SkeletonAnimation skeletonAnimation)
			{
				SetupSkin(defaultSkin, skeletonAnimation);
			}).AddTo(this);
		}
		base.Bord.Display(active: false);
		Tooltip.Display(display: false);
	}

	public void SetupSkin(CharacterAnimationSettings settings, SkeletonAnimation skeletonAnimation)
	{
		if ((bool)ObjectAnimation)
		{
			UnityEngine.Object.DestroyImmediate(ObjectAnimation.gameObject);
		}
		if (!(skeletonAnimation == null))
		{
			ObjectAnimation = UnityEngine.Object.Instantiate(skeletonAnimation, ViewRoot);
			ObjectAnimation.transform.localPosition = settings.Position;
			ObjectAnimation.transform.localScale = settings.Scale;
			animationMeshRenderer = ObjectAnimation.GetComponent<MeshRenderer>();
			ShaderFinder.UpdateMaterial(animationMeshRenderer.sharedMaterial);
			Material[] sharedMaterials = animationMeshRenderer.sharedMaterials;
			foreach (Material obj in sharedMaterials)
			{
				ShaderFinder.UpdateMaterial(obj);
				obj.color = Color.white;
			}
			collider.points = settings.ColliderPoints;
		}
	}

	public override IObservable<Bounds> GetBounds()
	{
		return Observable.Return(animationMeshRenderer.bounds);
	}

	public override void Highlight(HighlightType highlightType)
	{
		throw new NotImplementedException();
	}

	public override void SetSkin(int skinID)
	{
		if (this.skinID == skinID)
		{
			return;
		}
		this.skinID = skinID;
		skinLoadStream?.Dispose();
		skinLoadStream = base.Config.GetSkinSettings(skinID).SelectMany((CharacterAnimationSettings skin) => from animation in characterSkinLoader.Load(skinID)
			select (skin: skin, animation: animation)).Subscribe(delegate((CharacterAnimationSettings skin, SkeletonAnimation animation) pair)
		{
			SetupSkin(pair.skin, pair.animation);
		}, delegate(Exception exception)
		{
			exception.LogException();
		})
			.AddTo(this);
	}

	public override void SetView(int viewNumber)
	{
		if (!(ObjectAnimation == null) && viewNumber < ObjectAnimation.Skeleton.Data.Skins.Items.Length)
		{
			Spine.Skin skin = ObjectAnimation.Skeleton.Data.Skins.Items[viewNumber];
			ObjectAnimation.Skeleton.SetSkin(skin.Name);
		}
	}

	public override void SetVisible(bool visible)
	{
		if (ObjectAnimation == null)
		{
			return;
		}
		ObjectAnimation.gameObject.SetActive(visible);
		collider.enabled = visible;
		foreach (Material item in animationMeshRenderer.sharedMaterials.Where((Material _mat) => _mat.HasProperty("_Color")))
		{
			Color color = item.color;
			color.a = 0f;
			item.color = color;
		}
	}

	protected virtual void OnDestroy()
	{
		clickStream?.Dispose();
		skinLoadStream?.Dispose();
	}

	public override string ToString()
	{
		return base.ToString() + " " + base.gameObject.name;
	}

	public override void SetStatus(EntityStatus status)
	{
		throw new NotImplementedException();
	}

	public override void UpdateBord()
	{
		throw new NotImplementedException();
	}
}
