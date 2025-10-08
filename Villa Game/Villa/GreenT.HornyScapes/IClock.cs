using System;

namespace GreenT.HornyScapes;

public interface IClock
{
	DateTime GetTime();

	DateTime GetDate();

	TimeSpan GetTimeEndDay();
}
