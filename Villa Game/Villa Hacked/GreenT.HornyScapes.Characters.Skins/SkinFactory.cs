using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinFactory : IFactory<SkinMapper, Skin>, IFactory
{
	protected readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private readonly ISaver saver;

	public SkinFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saveManager)
	{
		this.lockerFactory = lockerFactory;
		saver = saveManager;
	}

	public Skin Create(SkinMapper mapper)
	{
		CompositeLocker locker = LockerFactory.CreateFromParamsArray(mapper.preload_type, mapper.preload_value, lockerFactory, LockerSourceType.Skin);
		Skin skin = new Skin(mapper, locker);
		saver.Add(skin);
		return skin;
	}
}
