using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace GreenT.HornyScapes.Meta;

public class Room
{
	public int ID { get; }

	public Transform Transform { get; }

	public IDictionary<string, IRoomObject<BaseObjectConfig>> RoomObjectsDict { get; }

	public IEnumerable<IRoomObject<BaseObjectConfig>> RoomObjects => RoomObjectsDict.Values;

	public Room(int id, Transform roomParent, IDictionary<string, IRoomObject<BaseObjectConfig>> roomObjects = null)
	{
		ID = id;
		Transform = roomParent;
		RoomObjectsDict = roomObjects ?? new Dictionary<string, IRoomObject<BaseObjectConfig>>();
	}

	public IRoomObject<BaseObjectConfig> GetObjectOrDefault(int number)
	{
		return RoomObjectsDict.Values.FirstOrDefault((IRoomObject<BaseObjectConfig> _object) => _object.Config.Number == number);
	}

	public Bounds GetBounds()
	{
		IEnumerable<Bounds> source = from _view in RoomObjectsDict.Values.OfType<RoomObject>().SelectMany((RoomObject _value) => _value.Views)
			select _view.Renderer.bounds;
		Bounds bounds = source.First();
		float x = bounds.min.x;
		float x2 = bounds.max.x;
		float y = bounds.min.y;
		float y2 = bounds.max.y;
		foreach (Bounds item in source.Skip(1))
		{
			if (item.min.x < x)
			{
				x = item.min.x;
			}
			if (item.max.x > x2)
			{
				x2 = item.max.x;
			}
			if (item.min.y < y)
			{
				y = item.min.y;
			}
			if (item.max.y > y2)
			{
				y2 = item.max.y;
			}
		}
		Vector3 position = Transform.position;
		Vector3 size = new Vector3(x2 - x, y2 - y);
		return new Bounds(position, size);
	}
}
