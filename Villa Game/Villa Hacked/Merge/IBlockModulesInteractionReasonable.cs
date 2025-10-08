namespace Merge;

public interface IBlockModulesInteractionReasonable : IBlockModulesInteraction
{
	string GetReasonForCase(GIModuleType GIModuleType);
}
