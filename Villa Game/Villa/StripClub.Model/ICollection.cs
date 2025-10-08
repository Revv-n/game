using System.Collections;
using System.Collections.Generic;

namespace StripClub.Model;

public interface ICollection<TKey, TValue> : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
{
	TValue GetByKey(TKey key);

	bool ContainsKey(TKey key);
}
