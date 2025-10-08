using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes;

public class DisposeDictionary : Dictionary<int, IDisposable>
{
	public void ClearStreams()
	{
		foreach (IDisposable value in base.Values)
		{
			value?.Dispose();
		}
		Clear();
	}

	public void FreeStream(int id)
	{
		if (ContainsKey(id))
		{
			base[id]?.Dispose();
			Remove(id);
		}
	}
}
