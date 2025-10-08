using StripClub.Extensions;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferTimer : MonoView<GenericTimer>
{
	[SerializeField]
	private MonoTimer timer;

	private TimeHelper timeHelper;

	[Inject]
	public void Init(TimeHelper timeHelper)
	{
		this.timeHelper = timeHelper;
	}

	public override void Set(GenericTimer timer)
	{
		base.Set(timer);
		this.timer.Init(timer, timeHelper.UseCombineFormat);
	}
}
