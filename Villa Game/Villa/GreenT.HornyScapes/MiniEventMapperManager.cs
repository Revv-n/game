using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public class MiniEventMapperManager : SimpleManager<MiniEventMapper>
{
	private readonly Dictionary<string, MiniEventMapper> _eventMappers = new Dictionary<string, MiniEventMapper>();

	public MiniEventMapper GetMiniEventInfo(int id)
	{
		return collection.FirstOrDefault((MiniEventMapper _event) => _event.id == id);
	}

	public bool TryGetIdForBundleInfo(string bundleId, out int id)
	{
		MiniEventMapper value;
		bool result = _eventMappers.TryGetValue(bundleId, out value);
		id = value?.id ?? 0;
		return result;
	}

	public override void Add(MiniEventMapper entity)
	{
		_eventMappers.TryAdd(entity.bundle, entity);
		base.Add(entity);
	}

	public override void AddRange(IEnumerable<MiniEventMapper> entities)
	{
		foreach (MiniEventMapper entity in entities)
		{
			Add(entity);
		}
	}
}
