namespace GreenT.HornyScapes.MergeStore;

public class SectionCreateDataPreset
{
	public readonly SectionCreateData Regular;

	public readonly SectionCreateData Premium;

	public SectionCreateDataPreset(SectionCreateData regular, SectionCreateData premium)
	{
		Regular = regular;
		Premium = premium;
	}
}
