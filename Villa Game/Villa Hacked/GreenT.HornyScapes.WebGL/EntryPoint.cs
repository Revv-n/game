using GreenT.HornyScapes.Saves;
using GreenT.Net.User;

namespace GreenT.HornyScapes.WebGL;

public class EntryPoint : BaseEntryPoint
{
	public EntryPoint(GameController gameController, SaveController saveController, RestoreSessionProcessor restoreSessionProcessor)
		: base(gameController, saveController, restoreSessionProcessor)
	{
	}
}
