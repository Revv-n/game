using System.Collections.Generic;
using GreenT.Types;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class PushControllerCluster
{
	private readonly Dictionary<ContentType, PushController> _controllers;

	public PushControllerCluster(Dictionary<ContentType, PushController> controllers)
	{
		_controllers = controllers;
	}

	public PushController GetController(ContentType type)
	{
		return CollectionExtensions.GetValueOrDefault<ContentType, PushController>((IReadOnlyDictionary<ContentType, PushController>)_controllers, type);
	}
}
