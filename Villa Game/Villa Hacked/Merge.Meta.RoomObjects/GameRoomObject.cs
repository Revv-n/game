using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Meta.RoomObjects;
using UniRx;

namespace Merge.Meta.RoomObjects;

public abstract class GameRoomObject<T> : BaseRoomObject<T>, IGameRoomObject<T>, IRoomObject<T>, ITapable<IRoomObject<T>> where T : BaseObjectConfig
{
	protected Subject<IRoomObject<T>> onTap = (Subject<IRoomObject<T>>)(object)new Subject<IRoomObject<IRoomObject<T>>>();

	public override bool IsVisible
	{
		get
		{
			bool num = base.Config.TrashBehaviour && !Data.IsRewarded;
			bool flag = base.Config.PresetBehaviour || base.Config.StaticBehaviour;
			bool flag2 = base.Config.NormalBehaviour && Data.IsRewarded;
			return num || flag || flag2;
		}
	}

	public ReactiveProperty<int> OnViewChanged { get; private set; } = new ReactiveProperty<int>();


	public RoomStateData Data { get; protected set; }

	public virtual IObservable<IRoomObject<T>> OnTap()
	{
		return Observable.AsObservable<IRoomObject<T>>((IObservable<IRoomObject<T>>)onTap);
	}

	public virtual void Init(RoomStateData data, T config)
	{
		base.Init(config);
		Data = data;
		OnViewChanged.Value = Data.SelectedSkin;
		SetView(Data.SelectedSkin);
		SetStatus(Data.Status);
	}

	public override void SetStatus(EntityStatus status)
	{
		Data.Status = status;
		bool isVisible = IsVisible;
		SetVisible(isVisible);
		UpdateBord();
	}

	public override void UpdateBord()
	{
		base.Bord.Display(Data.IsInProgress || Data.Complete);
		base.Bord.SetCollectable(Data.Complete);
	}

	protected virtual bool IsClickAllowed()
	{
		if (Data.IsRewarded)
		{
			if (!base.Config.NormalBehaviour)
			{
				return base.Config.PresetBehaviour;
			}
			return true;
		}
		return false;
	}

	protected virtual void OnDestroy()
	{
		((Subject<IRoomObject<IRoomObject<T>>>)(object)onTap).OnCompleted();
		((Subject<IRoomObject<IRoomObject<T>>>)(object)onTap).Dispose();
		OnViewChanged.Dispose();
	}
}
