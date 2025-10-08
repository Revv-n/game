using System;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public interface IRoomObject<out T> where T : BaseObjectConfig
{
	T Config { get; }

	RoomObjectBord Bord { get; }

	bool IsVisible { get; }

	IObservable<Bounds> GetBounds();

	void SetSkin(int skinID);

	void SetStatus(EntityStatus status);

	void SetView(int viewNumber);

	void SetVisible(bool visible);
}
