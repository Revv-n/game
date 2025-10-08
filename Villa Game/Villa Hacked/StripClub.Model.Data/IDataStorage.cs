using System.Collections.Generic;
using UnityEngine;

namespace StripClub.Model.Data;

public interface IDataStorage<T> where T : Object
{
	IEnumerable<K> GetData<K>() where K : T;
}
