using UnityEngine;

namespace Merge.Core.Masters;

public interface IPointerController
{
	void AtMouseDown(Vector3 position);

	void AtMouseUp(Vector3 position);
}
