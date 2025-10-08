using System;
using System.Linq;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Utilities;
using Spine;
using Spine.Unity;
using UniRx;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class JarvisAnimatedRoomObject : BaseRoomObject<AnimatedObjectConfig>
{
	private MeshRenderer animationMeshRenderer;

	public SkeletonAnimation ObjectAnimation { get; private set; }

	public override bool IsVisible => base.gameObject.activeSelf;

	public override void Init(AnimatedObjectConfig config)
	{
		base.Init(config);
		if (config.AnimationSettings.SkeletonAnimation != null)
		{
			SetupSkin(config.AnimationSettings);
		}
		base.Bord.Display(active: false);
	}

	public void SetupSkin(RoomObjectAnimationSettings animationSettings)
	{
		if ((bool)ObjectAnimation)
		{
			UnityEngine.Object.Destroy(ObjectAnimation.gameObject);
		}
		if (!(animationSettings.SkeletonAnimation == null))
		{
			ObjectAnimation = UnityEngine.Object.Instantiate(animationSettings.SkeletonAnimation, ViewRoot);
			ObjectAnimation.transform.localPosition = animationSettings.Position;
			ObjectAnimation.transform.localScale = animationSettings.Scale;
			animationMeshRenderer = ObjectAnimation.GetComponent<MeshRenderer>();
			ShaderFinder.UpdateMaterial(animationMeshRenderer.sharedMaterial);
			Material[] sharedMaterials = animationMeshRenderer.sharedMaterials;
			foreach (Material obj in sharedMaterials)
			{
				ShaderFinder.UpdateMaterial(obj);
				obj.color = Color.white;
			}
		}
	}

	public override IObservable<Bounds> GetBounds()
	{
		return Observable.Return<Bounds>(animationMeshRenderer.bounds);
	}

	public override void SetVisible(bool visible)
	{
		if (ObjectAnimation == null)
		{
			return;
		}
		ObjectAnimation.gameObject.SetActive(visible);
		foreach (Material item in animationMeshRenderer.sharedMaterials.Where((Material _mat) => _mat.HasProperty("_Color")))
		{
			Color color = item.color;
			color.a = 0f;
			item.color = color;
		}
	}

	public override void SetView(int viewNumber)
	{
		if (!(ObjectAnimation == null) && viewNumber < ObjectAnimation.Skeleton.Data.Skins.Items.Length)
		{
			Spine.Skin skin = ObjectAnimation.Skeleton.Data.Skins.Items[viewNumber];
			ObjectAnimation.Skeleton.SetSkin(skin.Name);
		}
	}

	public override void SetSkin(int skinID)
	{
		throw new NotImplementedException();
	}

	public override void Highlight(HighlightType highlightType)
	{
		throw new NotImplementedException();
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
