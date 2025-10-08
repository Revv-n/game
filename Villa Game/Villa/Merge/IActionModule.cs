namespace Merge;

public interface IActionModule
{
	bool IsActionEnable { get; }

	int ActionPriority { get; }
}
