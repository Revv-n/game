using System;

namespace StripClub.Model.Data;

public interface ISaleTime
{
	DateTime StartTime { get; }

	DateTime ExpirationTime { get; }

	bool IsInclude(DateTime currentTime);
}
