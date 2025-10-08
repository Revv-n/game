using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public abstract class LotFactory<TMapper, TLot> : IFactory<TMapper, TLot>, IFactory where TMapper : LotMapper where TLot : Lot
{
	protected readonly LockerFactory lockerFactory;

	protected readonly ISaver saver;

	protected readonly IPurchaseProcessor purchaseProcessor;

	public LotFactory(LockerFactory lockerFactory, ISaver saver, IPurchaseProcessor purchaseProcessor)
	{
		this.lockerFactory = lockerFactory;
		this.saver = saver;
		this.purchaseProcessor = purchaseProcessor;
	}

	public abstract TLot Create(TMapper param);

	protected CompositeLocker CreateCompositeLocker(TMapper mapper, out EqualityLocker<int> countLocker, LockerSourceType sourceType)
	{
		ILocker[] array = new ILocker[mapper.unlock_type.Length + 1];
		for (int i = 0; i != mapper.unlock_type.Length; i++)
		{
			ILocker locker = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], sourceType);
			array[i] = locker;
		}
		countLocker = new EqualityLocker<int>(0, mapper.buy_times, (mapper.buy_times > 0) ? Restriction.Max : Restriction.Min);
		array[mapper.unlock_type.Length] = countLocker;
		return new CompositeLocker(array);
	}
}
