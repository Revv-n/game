using UnityEngine;

namespace Merge.Core.Masters;

public interface IClickController
{
	void AtClick(Vector3 position, Vector3 downPosition);
}
