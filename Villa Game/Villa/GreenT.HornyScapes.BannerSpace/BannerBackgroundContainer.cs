using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerBackgroundContainer : MonoBehaviour
{
	[SerializeField]
	private Image _staticBackground;

	public Image StaticBackground => _staticBackground;
}
