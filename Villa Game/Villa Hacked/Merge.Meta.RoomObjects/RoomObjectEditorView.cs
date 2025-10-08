using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta.RoomObjects;
using StripClub.UI;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class RoomObjectEditorView : MonoBehaviour, IView<RoomObjectViewInfo>, IView
{
	[SerializeField]
	private List<SpriteRenderer> spriters;

	[SerializeField]
	private PolygonCollider2D col;

	[SerializeField]
	private Animation beforeChangeAnimation;

	[SerializeField]
	private Animation afterChangeAnimation;

	[SerializeField]
	private bool disableCollision;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Material blendMaterial;

	public SpriteRenderer this[int skin] => spriters[skin];

	public PolygonCollider2D Collider => col;

	public int SelectedSkin { get; private set; }

	public bool AllSkinsSelected { get; private set; }

	public int Order { get; private set; }

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	public Animation AfterChangeAnimation
	{
		get
		{
			return beforeChangeAnimation;
		}
		private set
		{
			beforeChangeAnimation = value;
		}
	}

	public Animation BeforeChangeAnimation
	{
		get
		{
			return afterChangeAnimation;
		}
		private set
		{
			afterChangeAnimation = value;
		}
	}

	public bool DisableCollision
	{
		get
		{
			return disableCollision;
		}
		private set
		{
			disableCollision = value;
		}
	}

	public Material Material => material;

	public Material BlendMaterial => blendMaterial;

	public void Set(RoomObjectViewInfo info)
	{
		foreach (RoomObjectViewParameters skinInfo in info.SkinInfos)
		{
			this[skinInfo.SkinID].SetOrder(info.Order);
			this[skinInfo.SkinID].transform.localPosition = skinInfo.Offset;
			this[skinInfo.SkinID].transform.localScale = skinInfo.Scale;
			if (skinInfo != null)
			{
				RoomObjectViewParameters roomObjectViewParameters = skinInfo;
				this[skinInfo.SkinID].sprite = roomObjectViewParameters.Sprite;
			}
		}
		base.transform.localPosition = info.Position;
		base.transform.localScale = info.LocalScale;
		col.points = info.PolygonPoints;
		Order = info.Order;
		if (info.Material != null)
		{
			material = info.Material;
		}
		foreach (SpriteRenderer spriter in spriters)
		{
			spriter.material = material;
		}
		if (info.BlendMaterial != null)
		{
			blendMaterial = info.BlendMaterial;
		}
		AfterChangeAnimation = info.AfterChangeAnimation;
		BeforeChangeAnimation = info.BeforeChangeAnimation;
		DisableCollision = info.DisableCollision;
	}

	public void SetActiveSkin(int skinID)
	{
		SelectedSkin = skinID;
		spriters.ForEach(delegate(SpriteRenderer x)
		{
			x.gameObject.SetActive(value: false);
		});
		spriters[skinID].gameObject.SetActive(value: true);
		AllSkinsSelected = false;
	}

	public void ShowAllSkins()
	{
		spriters.ForEach(delegate(SpriteRenderer x)
		{
			x.gameObject.SetActive(value: true);
		});
		AllSkinsSelected = true;
	}

	public void SetOrder(int order)
	{
		spriters.ForEach(delegate(SpriteRenderer x)
		{
			x.SetOrder(order);
		});
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}
}
