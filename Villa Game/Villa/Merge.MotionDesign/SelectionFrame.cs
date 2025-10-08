using UnityEngine;

namespace Merge.MotionDesign;

public class SelectionFrame : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] renderers;

	[SerializeField]
	private Sprite[] mergablePack;

	[SerializeField]
	private Sprite[] finalPack;

	public void SetSpritePack(bool mergable)
	{
		Sprite[] array = (mergable ? mergablePack : finalPack);
		for (int i = 0; i < array.Length; i++)
		{
			renderers[i].sprite = array[i];
		}
	}
}
