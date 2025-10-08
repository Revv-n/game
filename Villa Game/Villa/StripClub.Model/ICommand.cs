namespace StripClub.Model;

public interface ICommand
{
	void Apply();

	void Undo();
}
