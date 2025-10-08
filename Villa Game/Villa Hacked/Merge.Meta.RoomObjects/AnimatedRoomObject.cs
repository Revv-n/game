using System;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Utilities;
using Spine;
using Spine.Unity;
using UniRx;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class AnimatedRoomObject : GameRoomObject<AnimatedObjectConfig>
{
	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation animation;

	private MeshRenderer animationMeshRenderer;

	public SkeletonAnimation ObjectAnimation { get; private set; }

	public override void Init(RoomStateData data, AnimatedObjectConfig config)
	{
		DisplaySkin(config.AnimationSettings);
		SetupAppearanceAnimation(config);
		base.Init(data, config);
	}

	private void SetupAppearanceAnimation(AnimatedObjectConfig config)
	{
		if (config.ShowAnimation == null)
		{
			return;
		}
		GreenT.HornyScapes.Animations.Animation animation = UnityEngine.Object.Instantiate(config.ShowAnimation, base.transform);
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

	public void DisplaySkin(RoomObjectAnimationSettings settings)
	{
		if ((bool)ObjectAnimation)
		{
			UnityEngine.Object.Destroy(ObjectAnimation.gameObject);
		}
		ObjectAnimation = UnityEngine.Object.Instantiate(settings.SkeletonAnimation, ViewRoot);
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
		UpdateSpineAnimation(animationMeshRenderer);
	}

	private void UpdateSpineAnimation(MeshRenderer animationMeshRenderer)
	{
		SpineObjectAnimation[] componentsInChildren = base.transform.GetComponentsInChildren<SpineObjectAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SpineRenderer = animationMeshRenderer;
		}
	}

	public override IObservable<Bounds> GetBounds()
	{
		return Observable.Return<Bounds>(animationMeshRenderer.bounds);
	}

	public override void SetView(int viewNumber)
	{
		if (ObjectAnimation == null)
		{
			return;
		}
		try
		{
			Spine.Skin skin = ObjectAnimation.Skeleton.Data.Skins.Items[0];
			ObjectAnimation.Skeleton.SetSkin(skin.Name);
			base.Data.SelectedSkin = viewNumber;
		}
		catch (Exception innerException)
		{
			innerException.SendException(this?.ToString() + " exception on applying skin");
		}
	}

	public override void SetVisible(bool visible)
	{
		if (ObjectAnimation == null)
		{
			return;
		}
		if (base.Data.Status != EntityStatus.Rewarded)
		{
			visible = false;
		}
		ObjectAnimation.gameObject.SetActive(visible);
		if (!visible)
		{
			return;
		}
		foreach (Material item in animationMeshRenderer.sharedMaterials.Where((Material _mat) => _mat.HasProperty("_Color")))
		{
			Color color = item.color;
			color.a = 0f;
			item.color = color;
		}
		animation.Play();
	}

	public override void SetSkin(int skinID)
	{
		throw new NotImplementedException();
	}

	public override void Highlight(HighlightType highlightType)
	{
		throw new NotImplementedException();
	}
}
