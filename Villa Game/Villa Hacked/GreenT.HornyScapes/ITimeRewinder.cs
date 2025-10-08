using System;

namespace GreenT.HornyScapes;

public interface ITimeRewinder
{
	void Rewind(TimeSpan time);

	void Reset();
}
