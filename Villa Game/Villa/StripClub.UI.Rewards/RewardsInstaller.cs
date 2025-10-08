using GreenT.HornyScapes.UI;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class RewardsInstaller : MonoInstaller<RewardsInstaller>
{
	[SerializeField]
	private Transform container;

	[SerializeField]
	private CardResourceView resourceViewPrefab;

	[SerializeField]
	private CardView cardViewPrefab;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<CardView>().FromComponentInNewPrefab(cardViewPrefab).UnderTransform(container);
		base.Container.BindIFactory<CardResourceView>().FromComponentInNewPrefab(resourceViewPrefab).UnderTransform(container);
	}
}
