using StripClub.Model;

namespace GreenT.HornyScapes.MergeStore;

public class CreateData
{
	public readonly bool UsePremium;

	public readonly int ID;

	public readonly string Bundle;

	public readonly ILocker Lock;

	private readonly SectionCreateData _premiumSectionCreateData;

	public SectionCreateData RegularSectionCreateData { get; }

	public SectionCreateData PremiumSectionCreateData
	{
		get
		{
			if (!UsePremium)
			{
				return null;
			}
			return _premiumSectionCreateData;
		}
	}

	public CreateData(int id, string bundle, ILocker locker, SectionCreateData regularSectionCreateData, SectionCreateData premiumSectionCreateData, bool usePremium)
	{
		ID = id;
		Bundle = bundle;
		Lock = locker;
		UsePremium = usePremium;
		RegularSectionCreateData = regularSectionCreateData;
		_premiumSectionCreateData = premiumSectionCreateData;
	}
}
