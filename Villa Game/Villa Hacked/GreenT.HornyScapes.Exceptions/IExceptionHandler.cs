using System;

namespace GreenT.HornyScapes.Exceptions;

public interface IExceptionHandler
{
	void Handle(string reason);

	void Handle(Exception ex);

	void Handle(Exception innerEx, string reason);
}
