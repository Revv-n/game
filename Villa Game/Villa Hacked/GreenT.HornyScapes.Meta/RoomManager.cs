using System;
using System.Linq;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Model.Collections;
using GreenT.Types;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Meta;

public class RoomManager : SimpleManager<Room>
{
	public Room GetRoomOrDefault(int id)
	{
		return collection.FirstOrDefault((Room _room) => _room.ID == id);
	}

	public bool IsObjectSet(CompositeIdentificator id, bool isGirlId = false)
	{
		Room room = (isGirlId ? FindRoomWithGirl(id.GetParameter(0)) : GetRoomOrDefault(id.GetParameter(0)));
		if (room == null)
		{
			return false;
		}
		int findId = id.GetParameter((!isGirlId) ? 1 : 0);
		return room.RoomObjects.Any((IRoomObject<BaseObjectConfig> _object) => _object.Config.Number == findId);
	}

	public void Init()
	{
		collection.Clear();
	}

	public IGameRoomObject<BaseObjectConfig> GetRoomObject(CompositeIdentificator id)
	{
		return GetObject(id) as IGameRoomObject<BaseObjectConfig>;
	}

	public IRoomObject<BaseObjectConfig> GetObject(CompositeIdentificator id)
	{
		if (collection.Count == 0)
		{
			return null;
		}
		try
		{
			bool flag = id.Identificators.Length == 1;
			Room room = (flag ? FindRoomWithGirl(id.GetParameter(0)) : GetRoomOrDefault(id.GetParameter(0)));
			return FindRoomObject(id, room, flag);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public CharacterObject GetCharacterObject(int id)
	{
		try
		{
			return collection.SelectMany((Room _room) => _room.RoomObjects).OfType<CharacterObject>().First((CharacterObject _spine) => _spine.Config.CharacterID.Equals(id));
		}
		catch (InvalidOperationException innerException)
		{
			throw innerException.SendException("Can't find Character:" + id + " among house objects");
		}
	}

	public IRoomObject<BaseObjectConfig> FindRoomObject(CompositeIdentificator id, Room room, bool isGirlId)
	{
		IRoomObject<BaseObjectConfig> objectOrDefault = room.GetObjectOrDefault(id.GetParameter((!isGirlId) ? 1 : 0));
		if (objectOrDefault == null)
		{
			string name = GetType().Name;
			CompositeIdentificator compositeIdentificator = id;
			throw new ArgumentOutOfRangeException("id", name + ": No object with ID: " + compositeIdentificator.ToString()).LogException();
		}
		return objectOrDefault;
	}

	private Room FindRoomWithGirl(int girlId)
	{
		return collection.FirstOrDefault((Room _room) => _room.RoomObjects.Any((IRoomObject<BaseObjectConfig> obj) => obj.Config.Number == girlId)) ?? throw new Exception().SendException($"{GetType().Name}: doesn't have room with girl: {girlId}");
	}

	public bool IsCharacterObjectSet(int girlId, out CharacterObject characterObject)
	{
		characterObject = collection.SelectMany((Room _room) => _room.RoomObjects).OfType<CharacterObject>().FirstOrDefault((CharacterObject _charObject) => _charObject.Config.CharacterID == girlId);
		return characterObject != null;
	}

	internal object GetObject(object iD)
	{
		throw new NotImplementedException();
	}
}
