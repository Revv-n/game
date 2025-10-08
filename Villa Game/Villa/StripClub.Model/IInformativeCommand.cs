namespace StripClub.Model;

public interface IInformativeCommand : ICommand
{
	bool IsApplicable();

	bool IsApplied();
}
