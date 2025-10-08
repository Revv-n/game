namespace GreenT.HornyScapes.Stories;

public class Phrase
{
	public int Step { get; }

	public int[,] CharactersVisible { get; }

	public int CharacterID { get; }

	public string Text { get; private set; }

	public string Name { get; private set; }

	public bool IsComplete { get; private set; }

	public Phrase(int stepID, int[,] characters_visible, int characterID, string text, string name)
	{
		Step = stepID;
		CharactersVisible = characters_visible;
		CharacterID = characterID;
		Text = text;
		Name = name;
	}

	public void Complete()
	{
		IsComplete = true;
	}

	public void Uncomplete()
	{
		IsComplete = false;
	}
}
