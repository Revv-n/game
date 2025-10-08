using System;
using DG.Tweening;
using GreenT.HornyScapes.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public abstract class BaseRoomObject<T> : MonoBehaviour, IRoomObject<T> where T : BaseObjectConfig
{
	private const string NAME_TEMPLATE = "RoomObject ({0})";

	[SerializeField]
	private RoomObjectBord bord;

	public Transform ViewRoot;

	public Sequence AnimationSequence { get; private set; }

	public T Config { get; private set; }

	public RoomObjectBord Bord => bord;

	public abstract bool IsVisible { get; }

	public virtual void Init(T config)
	{
		Config = config;
		base.gameObject.name = $"RoomObject ({config.ObjectName})";
		Bord.transform.position = config.BordPosition;
	}

	protected void SetConfig(T config)
	{
		Config = config;
	}

	public abstract void Highlight(HighlightType highlightType);

	public abstract void SetStatus(EntityStatus status);

	public abstract void UpdateBord();

	public abstract void SetSkin(int skinID);

	public abstract void SetView(int viewNumber);

	public abstract void SetVisible(bool visible);

	public abstract IObservable<Bounds> GetBounds();
}
