using System.Linq;
using System.Numerics;

namespace StripClub.Extensions;

public static class BigIntegerExtension
{
	public static BigInteger Multiply(this BigInteger left, decimal right)
	{
		(BigInteger, BigInteger) tuple = Fraction(right);
		return left * tuple.Item1 / tuple.Item2;
	}

	public static (BigInteger numerator, BigInteger denominator) Fraction(decimal d)
	{
		int[] bits = decimal.GetBits(d);
		BigInteger item = (1 - ((bits[3] >> 30) & 2)) * (((BigInteger)(uint)bits[2] << 64) | ((BigInteger)(uint)bits[1] << 32) | (uint)bits[0]);
		BigInteger item2 = BigInteger.Pow(10, (bits[3] >> 16) & 0xFF);
		return (numerator: item, denominator: item2);
	}

	public static int Pow(this int bas, int exp)
	{
		return Enumerable.Repeat(bas, exp).Aggregate(1, (int a, int b) => a * b);
	}

	public static BigInteger Pow(this double num, double pow)
	{
		return num.Pow(pow);
	}
}
