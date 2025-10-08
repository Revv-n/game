using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.ToolTips;
using GreenT.Utilities;
using Merge.Meta.RoomObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Merge.Meta;

public class RoomObjectView : MonoBehaviour, IRayClickable, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private SpriteRenderer sr;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private PolygonCollider2D polygonCollider;

	[SerializeField]
	private ToolTipOpener toolTipOpener;

	[SerializeField]
	private RoomObjectAnimationGroup afterChangeAnimationGroup;

	[SerializeField]
	private RoomObjectAnimationGroup beforeChangeAnimationGroup;

	private Func<bool> clickEnableDelegate;

	private RoomObjectViewParameters activeSkin;

	private RoomObjectViewInfo viewInfo;

	private Vector2 basePosition;

	private Vector2 baseScale;

	private int nextSkinId = 1;

	private RoomManager house;

	public SpriteRenderer Renderer => sr;

	public Animator Anim => anim;

	bool IRayClickable.IsRayClickEnable => clickEnableDelegate();

	RayClickOrder IRayClickable.RayClickOrder => new RayClickOrder(sr.sortingOrder, 1);

	public Animation AfterChangeAnimation => afterChangeAnimationGroup;

	public Animation BeforeChangeAnimation => beforeChangeAnimationGroup;

	public event Action OnClick;

	[Inject]
	private void Construct(RoomManager house)
	{
		this.house = house;
	}

	public void Init(Func<bool> clickEnableDelegate, Action clickCallback, RoomObjectViewInfo viewInfo)
	{
		OnClick += clickCallback;
		this.clickEnableDelegate = clickEnableDelegate;
		this.viewInfo = viewInfo;
		sr.sortingOrder = viewInfo.Order;
		if ((bool)polygonCollider)
		{
			polygonCollider.points = viewInfo.PolygonPoints;
			polygonCollider.enabled = !viewInfo.DisableCollision;
		}
		basePosition = viewInfo.Position;
		baseScale = viewInfo.LocalScale;
		if ((bool)viewInfo.ToolTipSettings)
		{
			toolTipOpener.Init(viewInfo.ToolTipSettings);
		}
		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		if ((bool)viewInfo.Material)
		{
			sr.material = viewInfo.Material;
		}
		if ((bool)viewInfo.Material)
		{
			ShaderFinder.UpdateMaterial(sr.sharedMaterial);
		}
		sr.SetPropertyBlock(propertyBlock);
	}

	public void SetView(int ViewNumber)
	{
		activeSkin = viewInfo.GetViewInfo(ViewNumber);
		if (activeSkin == null)
		{
			return;
		}
		sr.sprite = activeSkin.Sprite;
		if (sr.sprite != null)
		{
			toolTipOpener.SetActive(active: true);
		}
		else
		{
			if ((bool)polygonCollider)
			{
				polygonCollider.enabled = false;
			}
			toolTipOpener.SetActive(active: false);
		}
		base.transform.localPosition = basePosition + activeSkin.Offset;
		base.transform.localScale = baseScale * activeSkin.Scale;
		nextSkinId++;
	}

	public void PlayAnimation(HighlightType highlightType)
	{
		if ((bool)(UnityEngine.Object)(object)anim)
		{
			anim.Play(highlightType.ToString());
		}
	}

	void IRayClickable.AtRayClick()
	{
		this.OnClick?.Invoke();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnClick?.Invoke();
	}

	public void CreateAnimations(RoomObjectViewInfo info)
	{
		if ((bool)info.AfterChangeAnimation)
		{
			afterChangeAnimationGroup.SetAnimations(InitAnimations(UnityEngine.Object.Instantiate(info.AfterChangeAnimation, base.transform), "AfterChangeAnimation"));
		}
		if ((bool)info.BeforeChangeAnimation)
		{
			beforeChangeAnimationGroup.SetAnimations(InitAnimations(UnityEngine.Object.Instantiate(info.BeforeChangeAnimation, base.transform), "BeforeChangeAnimation"));
		}
	}

	private List<Animation> InitAnimations(Animation prefabInstance, string name)
	{
		List<Animation> list = new List<Animation>();
		prefabInstance.name = name;
		Animation[] components = prefabInstance.GetComponents<Animation>();
		foreach (Animation animation in components)
		{
			InitAnimation(animation);
			list.Add(animation);
			animation.Init();
		}
		return list;
	}

	private void InitAnimation(Animation animation)
	{
		if (!(animation is TransformAnimation transformAnimation))
		{
			if (animation is AnimationGroup animationGroup)
			{
				foreach (Animation animation2 in animationGroup.Animations)
				{
					InitAnimation(animation2);
				}
				return;
			}
			if (!(animation is SpriteBlendingAnimation spriteBlendingAnimation))
			{
				if (animation is ROFadeAnimation rOFadeAnimation)
				{
					rOFadeAnimation.SetHouse(house);
				}
			}
			else
			{
				sr.material = viewInfo.BlendMaterial;
				spriteBlendingAnimation.SpriteRenderer = sr;
				RoomObjectViewParameters roomObjectViewParameters = viewInfo.GetViewInfo(nextSkinId);
				spriteBlendingAnimation.TargetSprite = roomObjectViewParameters.Sprite.texture;
				ShaderFinder.UpdateMaterial(sr.material);
			}
		}
		else
		{
			transformAnimation.Transform = base.transform;
			transformAnimation.Renderer = sr;
		}
	}
}
