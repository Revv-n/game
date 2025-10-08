using UnityEngine;

namespace Merge.Core.Masters;

public interface IDragController
{
	void AtStartDrag(Vector3 position, Vector3 initDragPosition);

	void AtDrag(Vector3 position, Vector3 delta);

	void AtEndDrag(Vector3 position);
}
