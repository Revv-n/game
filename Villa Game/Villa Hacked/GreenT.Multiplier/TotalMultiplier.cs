using System;
using System.Numerics;
using UniRx;

namespace GreenT.Multiplier;

public class TotalMultiplier : IDisposable
{
	private ReactiveProperty<BigInteger> factor;

	private ReactiveProperty<byte> level;

	private IDisposable calcStream;

	public ReactiveProperty<BigInteger> Factor => factor;

	public IReadOnlyReactiveProperty<byte> Level => (IReadOnlyReactiveProperty<byte>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<byte>((IObservable<byte>)level);

	public BigInteger Coeffitient(byte level)
	{
		return BigInteger.Pow(5, level - 1);
	}

	public TotalMultiplier(BigInteger value)
	{
		factor = new ReactiveProperty<BigInteger>(value);
		level = new ReactiveProperty<byte>((byte)1);
		calcStream = ObservableExtensions.Subscribe<BigInteger>((IObservable<BigInteger>)factor, (Action<BigInteger>)delegate(BigInteger _factor)
		{
			level.Value = EvaluateMultiplierLevel(_factor);
		});
	}

	private byte EvaluateMultiplierLevel(BigInteger value)
	{
		byte b = 1;
		if (value > 50L)
		{
			double num = BigInteger.Log(value / 50, 5.0);
			BigInteger bigInteger = value % 50;
			b += (byte)num;
			if (bigInteger > 0L || num == Math.Floor(num))
			{
				b++;
			}
		}
		if (b > 50)
		{
			b = 50;
		}
		return b;
	}

	public BigInteger QuantityUpToNextLevel()
	{
		return 50 * Coeffitient((byte)(level.Value + 1)) + 1 - Factor.Value;
	}

	public float LevelProgressRatio()
	{
		BigInteger bigInteger = Coeffitient(level.Value);
		BigInteger bigInteger2 = ((level.Value > 1) ? Coeffitient((byte)(level.Value - 1)) : ((BigInteger)0));
		return (float)((double)(factor.Value - 50 * bigInteger2 - 1) / (double)(50 * (bigInteger - bigInteger2) - 1));
	}

	public void Add(int value, bool baseFactor = true)
	{
		BigInteger bigInteger = value;
		if (baseFactor)
		{
			bigInteger *= Coeffitient(level.Value);
		}
		ReactiveProperty<BigInteger> obj = factor;
		obj.Value += bigInteger;
	}

	public void Dispose()
	{
		calcStream?.Dispose();
	}
}
