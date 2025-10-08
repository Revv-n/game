using System;

namespace GreenT.HornyScapes.Exceptions;

public class TypeMismatchException : Exception
{
	public TypeMismatchException(Type current, Type target)
		: base($"Not possible to convert type {current} to type {target}")
	{
	}
}
