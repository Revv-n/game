using GreenT.HornyScapes.Saves;
using GreenT.Net.User;
using Zenject;

namespace GreenT.HornyScapes;

public class BaseEntryPoint : IInitializable
{
	private readonly RestoreSessionProcessor restoreSessionProcessor;

	protected GameController gameController;

	private readonly SaveController saveController;

	public BaseEntryPoint(GameController gameController, SaveController saveController, RestoreSessionProcessor restoreSessionProcessor)
	{
		this.gameController = gameController;
		this.saveController = saveController;
		this.restoreSessionProcessor = restoreSessionProcessor;
	}

	public virtual void Initialize()
	{
		saveController.Launch();
		gameController.Launch();
		restoreSessionProcessor.RestoreRequest();
	}
}
