using GreenT.Data;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceInitializer : IInitializable
{
	private readonly ISaver _saver;

	private readonly LastChanceManager _lastChanceManager;

	private readonly LastChanceController _lastChanceController;

	public LastChanceInitializer(ISaver saver, LastChanceManager lastChanceManager, LastChanceController lastChanceController)
	{
		_saver = saver;
		_lastChanceManager = lastChanceManager;
		_lastChanceController = lastChanceController;
	}

	public void Initialize()
	{
		_saver.Add(_lastChanceManager);
		_lastChanceController.Init();
	}
}
