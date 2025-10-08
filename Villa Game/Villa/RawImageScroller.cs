using UnityEngine;
using UnityEngine.UI;

public class RawImageScroller : MonoBehaviour
{
	[SerializeField]
	private bool isHorizontal = true;

	[SerializeField]
	private float speed = -1f;

	private RawImage rImg;

	private void Start()
	{
		rImg = GetComponent<RawImage>();
	}

	private void Update()
	{
		Rect uvRect = rImg.uvRect;
		if (isHorizontal)
		{
			uvRect.x += speed * Time.deltaTime;
		}
		else
		{
			uvRect.y += speed * Time.deltaTime;
		}
		rImg.uvRect = uvRect;
	}
}
