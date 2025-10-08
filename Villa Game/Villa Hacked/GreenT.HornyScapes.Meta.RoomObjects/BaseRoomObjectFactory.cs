using Merge;
using Merge.Meta.RoomObjects;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class BaseRoomObjectFactory<KConfig, TObject> : IFactory<KConfig, Transform, TObject>, IFactory where KConfig : BaseObjectConfig where TObject : GameRoomObject<KConfig>
{
	private readonly TObject roomObjectPrefab;

	private readonly DiContainer container;

	public BaseRoomObjectFactory(DiContainer container, TObject prefab)
	{
		this.container = container;
		roomObjectPrefab = prefab;
	}

	public TObject Create(KConfig config, Transform parent)
	{
		TObject val = container.InstantiatePrefabForComponent<TObject>((Object)roomObjectPrefab, parent);
		val.transform.SetDefault();
		RoomStateData data = new RoomStateData(config.name);
		val.Init(data, config);
		return val;
	}
}
