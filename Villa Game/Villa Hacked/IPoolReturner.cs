using System;

public interface IPoolReturner
{
	Action ReturnInPool { get; set; }
}
