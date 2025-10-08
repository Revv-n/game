using GreenT.HornyScapes.Monetization.Android.Erolabs;
using GreenT.HornyScapes.Monetization.Android.Harem;
using GreenT.HornyScapes.Monetization.Harem;
using GreenT.HornyScapes.Monetization.Webgl.Epocha;
using GreenT.HornyScapes.Monetization.Webgl.Nutaku;
using GreenT.HornyScapes.Monetization.Windows.Steam;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Monetization;

public class MonoMonetizationPopupInstaller : MonoInstaller<MonoMonetizationPopupInstaller>
{
	[SerializeField]
	protected Canvas canvasParent;

	[SerializeField]
	protected EpochaPopup webglEpochaPopupPrefab;

	[SerializeField]
	protected NutakuPopup webglNutakuPopupPrefab;

	[SerializeField]
	protected SteamPopup windowsSteamPopupPrefab;

	[SerializeField]
	protected HaremPopup webglHaremPopupPrefab;

	[SerializeField]
	protected ErolabsPopup androidErolabsPopupPrefab;

	[SerializeField]
	protected HaremChoosePaymentPopup androidHaremChoosePaymentPrefab;

	public override void InstallBindings()
	{
		BindFactory();
	}

	private void BindFactory()
	{
		base.Container.BindInterfacesAndSelfTo<MonetizationPopupFactory<SteamPopup>>().AsSingle().WithArguments(canvasParent, windowsSteamPopupPrefab);
	}
}
