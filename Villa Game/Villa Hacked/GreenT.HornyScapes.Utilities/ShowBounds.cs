using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Utilities;

public class ShowBounds : MonoBehaviour
{
	public List<SpriteRenderer> SpriteRenderers;

	private Bounds bounds;

	[EditorButton]
	public void CalculateBounds()
	{
		Bounds bounds = default(Bounds);
		for (int i = 0; i < SpriteRenderers.Count; i++)
		{
			Renderer renderer = SpriteRenderers[i];
			if (i == 0)
			{
				bounds = renderer.bounds;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		this.bounds = bounds;
		Debug.Log("bounds.center: " + this.bounds.center.ToString());
	}

	[EditorButton]
	public void GetChildrens()
	{
		SpriteRenderers.Clear();
		SpriteRenderers.AddRange(base.transform.GetComponentsInChildren<SpriteRenderer>());
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(bounds.center, 0.1f);
	}
}
