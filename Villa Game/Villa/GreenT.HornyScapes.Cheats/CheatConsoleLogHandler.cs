using System;
using GreenT.Cheats;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatConsoleLogHandler : IDisposable
{
	private ConsoleLogSettingsSO _logSettings;

	private ConsoleCanvas _consoleCanvas;

	public CheatConsoleLogHandler(ConsoleLogSettingsSO logSettings, ConsoleCanvas consoleCanvas)
	{
		_consoleCanvas = consoleCanvas;
		_logSettings = logSettings;
		Console.LogSettings = _logSettings;
		Application.logMessageReceived += _consoleCanvas.CheatConsole.HandleLog;
		Console.OnLogCollection += _consoleCanvas.CheatConsole.DisplayCollection;
	}

	public void Dispose()
	{
		Application.logMessageReceived -= _consoleCanvas.CheatConsole.HandleLog;
		Console.OnLogCollection -= _consoleCanvas.CheatConsole.DisplayCollection;
	}
}
