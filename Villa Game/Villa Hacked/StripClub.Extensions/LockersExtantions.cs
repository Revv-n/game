using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes;
using StripClub.Model;

namespace StripClub.Extensions;

public static class LockersExtantions
{
	public static TimeSpan GetEnableFromTimeLeft(this IEnumerable<ILocker> lockers)
	{
		TimeSpan result = new TimeSpan(0L);
		long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		EnableFromLocker enableFromLocker = (EnableFromLocker)lockers.FirstOrDefault((ILocker x) => x is EnableFromLocker);
		if (enableFromLocker != null)
		{
			long num2 = enableFromLocker.To - num;
			result = new TimeSpan(0, 0, (int)num2);
		}
		return result;
	}
}
