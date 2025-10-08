using UnityEngine;

namespace Merge;

public class GIGhost : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer sr;

	[SerializeField]
	private Transform sizeFitter;

	public SpriteRenderer IconRenderer => sr;

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	public static GIGhost CreateGhost(GIKey key, Transform root, float size = 1f)
	{
		return CreateGhost(IconProvider.GetGISprite(key), root, size);
	}

	public static GIGhost CreateGhost(Sprite sprite, Transform root, float size = 1f)
	{
		GIGhost gIGhost = Object.Instantiate(Resources.Load<GIGhost>("GameItemGhost"));
		gIGhost.transform.SetParent(root);
		gIGhost.sizeFitter.localScale = Vector3.one * size;
		gIGhost.sr.sprite = sprite;
		return gIGhost;
	}

	public static GIGhost CreateGhost(GameItem original)
	{
		GIGhost gIGhost = Object.Instantiate(Resources.Load<GIGhost>("GameItemGhost"));
		gIGhost.transform.position = original.transform.position;
		gIGhost.transform.SetParent(original.transform.parent);
		gIGhost.sizeFitter.localScale = Vector3.one * original.SizeMul;
		gIGhost.sr.sprite = original.Icon;
		gIGhost.sr.size = original.IconRenderer.size;
		gIGhost.sr.SetOrder(original.IconRenderer);
		return gIGhost;
	}

	public static GIGhost CreateGhost(SpriteRenderer original)
	{
		GIGhost gIGhost = Object.Instantiate(Resources.Load<GIGhost>("GameItemGhost"));
		gIGhost.transform.position = original.transform.position;
		gIGhost.transform.SetParent(original.transform.parent);
		gIGhost.sizeFitter.localScale = original.transform.localScale;
		gIGhost.sr.sprite = original.sprite;
		gIGhost.sr.size = original.size;
		gIGhost.sr.SetOrder(original);
		return gIGhost;
	}

	public static RectTransform CreateGhost(GameObject source)
	{
		GameObject obj = Object.Instantiate(source.gameObject);
		obj.name = "UIGhost (" + source.name + ")";
		RectTransform obj2 = obj.transform as RectTransform;
		obj2.SetParent(source.transform);
		obj2.localScale = Vector3.one;
		obj2.anchorMin = Vector2.zero;
		obj2.anchorMax = Vector2.one;
		obj2.sizeDelta = Vector3.zero;
		obj2.localPosition = Vector3.zero;
		obj.transform.SetParent(UIMaster.OverlayUiCanvas.transform);
		return obj.transform as RectTransform;
	}
}
