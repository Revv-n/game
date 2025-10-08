using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class GarantChanceFactory : IFactory<GarantChanceMapper, GarantChance>, IFactory
{
	public GarantChance Create(GarantChanceMapper mapper)
	{
		return new GarantChance(mapper.garant_id);
	}
}
