using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Level.UI;

public class LvlUpProgressAnimation : MonoBehaviour
{
	[SerializeField]
	private Image glow;

	[SerializeField]
	private Image progressBack;

	[SerializeField]
	private Sprite progressCompletedSprite;

	[SerializeField]
	private GameObject particles;

	private Sprite progressOriginalSprite;

	public void Init()
	{
		progressOriginalSprite = progressBack.sprite;
	}

	public void Show()
	{
		particles.SetActive(value: true);
		progressBack.sprite = progressCompletedSprite;
		glow.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		particles.SetActive(value: false);
		progressBack.sprite = progressOriginalSprite;
		glow.gameObject.SetActive(value: false);
	}
}
