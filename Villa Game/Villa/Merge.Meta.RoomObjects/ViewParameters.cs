using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

[Serializable]
public abstract class ViewParameters
{
	[SerializeField]
	private int skinID;

	[SerializeField]
	private Vector2 offset;

	[SerializeField]
	private Vector2 scale = Vector2.one;

	public int SkinID => skinID;

	public Vector2 Offset => offset;

	public Vector2 Scale => scale;

	public ViewParameters(int skinID, Vector2 offset, Vector2 scale)
	{
		this.skinID = skinID;
		this.offset = offset;
		this.scale = scale;
	}
}
[Serializable]
public class ViewParameters<T> where T : ViewParameters
{
	[SerializeField]
	private ToolTipSettings toolTipSettings;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation beforeChangeAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation afterChangeAnimation;

	[SerializeField]
	private Vector2 position;

	[SerializeField]
	private Vector2 localScale;

	[SerializeField]
	private int order;

	[SerializeField]
	private bool disableCollision;

	[SerializeField]
	private Vector2[] points;

	[SerializeField]
	private List<T> skinInfos;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Material blendMaterial;

	public bool DisableCollision => disableCollision;

	public Vector2[] PolygonPoints => points;

	public int Order => order;

	public Vector2 Position => position;

	public Vector2 LocalScale => localScale;

	public List<T> SkinInfos => skinInfos;

	public GreenT.HornyScapes.Animations.Animation BeforeChangeAnimation => beforeChangeAnimation;

	public GreenT.HornyScapes.Animations.Animation AfterChangeAnimation => afterChangeAnimation;

	public ToolTipSettings ToolTipSettings => toolTipSettings;

	public Material Material => material;

	public Material BlendMaterial
	{
		get
		{
			return blendMaterial;
		}
		set
		{
			blendMaterial = value;
		}
	}

	public ViewParameters(Vector2 position, Vector2 localScale, int order, Vector2[] points, List<T> skinInfos, GreenT.HornyScapes.Animations.Animation beforeChangeAnimation, GreenT.HornyScapes.Animations.Animation afterChangeAnimation, Material material, Material blendMaterial, bool disableCollision)
	{
		this.position = position;
		this.localScale = localScale;
		this.order = order;
		this.points = points;
		this.skinInfos = skinInfos;
		this.beforeChangeAnimation = beforeChangeAnimation;
		this.afterChangeAnimation = afterChangeAnimation;
		this.material = material;
		this.blendMaterial = blendMaterial;
		this.disableCollision = disableCollision;
	}

	public T GetViewInfo(int viewNumber)
	{
		return skinInfos.FirstOrDefault((T x) => x.SkinID == viewNumber);
	}
}
