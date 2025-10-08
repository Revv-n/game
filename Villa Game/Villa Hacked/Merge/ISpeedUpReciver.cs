using System;

namespace Merge;

public interface ISpeedUpReciver
{
	float SpeedUpMultiplyer { get; }

	event Action<bool> OnSpeedUpChange;

	void AddSpeedUpSource(ISpeedUpSource source);

	void RemoveSpeedUpSource(ISpeedUpSource source);

	void RemoveSpeedUpSourcesWithKey(string key);
}
