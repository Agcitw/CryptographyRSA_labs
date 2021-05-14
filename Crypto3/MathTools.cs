using System;
using System.Numerics;

namespace Crypto3
{
	public static class MathTools
	{
		#region 3.2

		private static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
		{
			var random = new Random();
			var bytes = max.ToByteArray();
			random.NextBytes(bytes);
			bytes[bytes.Length - 1] &= 0x7F;
			var value = new BigInteger(bytes);
			var result = value % (max - min + 1) + min;
			return result;
		}

		public static class Fermat
		{
			public static bool Execute(BigInteger p, int iterations)
			{
				var exponentBigInteger = p - BigInteger.One;
				var iterator = 1;
				var prime = true;
				while (iterator < iterations && prime)
				{
					if (BigInteger.ModPow(GenerateRandomBigInteger(1, p), exponentBigInteger, p) != 1)
						prime = false;
					iterator++;
				}

				return prime;
			}
		}

		public static class MillerRabin
		{
			public static bool Execute(BigInteger p, int iterations)
			{
				BigInteger k, q;
				var num = p - BigInteger.One;
				var y = BigInteger.ModPow(210, num, p);
				if (y != 1) return false;
				for (k = BigInteger.Zero, q = num; q.IsEven; k++, q >>= 1)
				{
				}

				var isPrime = true;
				for (var r = 0; r < iterations && isPrime; r++)
				{
					var x = GenerateRandomBigInteger(BigInteger.One, num);
					isPrime = MillerRabinInner(p, x, q, k);
				}

				return isPrime;
			}

			private static bool MillerRabinInner(BigInteger n, BigInteger x, BigInteger q, BigInteger k)
			{
				var num = n - BigInteger.One;
				var y = BigInteger.ModPow(x, q, n);
				if (y == 1 || y == num)
					return true;
				for (BigInteger i = 1; i < k; i++)
				{
					y = BigInteger.ModPow(y, 2, n);
					if (y == num)
						return true;
					if (y == 1)
						return false;
				}

				return false;
			}
		}

		public static class SolovayStrassen
		{
			public static bool Execute(BigInteger p, int iterations)
			{
				if (p.IsZero || p.IsOne)
					return false;
				if (p == 2 || p == 3)
					return true;
				if (p.IsEven)
					return false;
				var pSub1 = p - BigInteger.One;
				var pSub1Div2 = pSub1 >> 1;
				for (var i = 0; i < iterations; i++)
				{
					var a = GenerateRandomBigInteger(BigInteger.One, p);
					var jacobi = Jacobi(a, p);
					if (jacobi == 0)
						return false;
					var exponentResult = BigInteger.ModPow(a, pSub1Div2, p);
					if (exponentResult == pSub1)
						exponentResult = BigInteger.MinusOne;
					if (jacobi != exponentResult)
						return false;
				}

				return true;
			}
		}

		#endregion

		#region 3.1

		public static BigInteger EuclidAlgorithm(BigInteger firstS, BigInteger secondS)
		{
			if (firstS == 0 || secondS == 0)
				return 0;
			if (firstS < 0 || secondS < 0)
				return -1;
			while (true)
			{
				var r = firstS % secondS;
				if (r == 0) break;
				firstS = secondS;
				secondS = r;
			}

			return secondS;
		}

		public static EuclidExtendedSolution EuclidAlgorithmExtended(BigInteger a, BigInteger b)
		{
			BigInteger
				x0 = BigInteger.One,
				xn = BigInteger.One,
				y0 = BigInteger.Zero,
				yn = BigInteger.Zero,
				x1 = BigInteger.Zero,
				y1 = BigInteger.One,
				r = a % b;
			while (r > 0)
			{
				var q = a / b;
				xn = x0 - q * x1;
				yn = y0 - q * y1;
				x0 = x1;
				y0 = y1;
				x1 = xn;
				y1 = yn;
				a = b;
				b = r;
				r = a % b;
			}

			return new EuclidExtendedSolution(xn, yn, b);
		}

		public readonly struct EuclidExtendedSolution
		{
			public BigInteger X { get; }
			public BigInteger Y { get; }
			public BigInteger D { get; }

			public EuclidExtendedSolution(BigInteger x, BigInteger y, BigInteger d)
			{
				X = x;
				Y = y;
				D = d;
			}
		}

		public static BigInteger FastExp(BigInteger x, BigInteger y, BigInteger p)
		{
			var res = BigInteger.One;
			x %= p;
			if (x == 0)
				return BigInteger.Zero;
			while (y > 0)
			{
				if ((y & 1) != 0)
					res = res * x % p;
				y >>= 1;
				x = x * x % p;
			}

			return res;
		}

		public static BigInteger EulerFunc(BigInteger m)
		{
			if (m < 2)
				throw new ArgumentException("m must be >= 2.");
			var res = 0;
			for (var i = 1; i < m; ++i)
				if (EuclidAlgorithm(i, m) == 1)
					res++;
			return res;
		}

		public static BigInteger Legendre(BigInteger a, BigInteger p)
		{
			BigInteger result;
			var two = new BigInteger(2);
			if (a == 0) return BigInteger.Zero;
			if (a == 1) return BigInteger.One;
			if (a % two == 0)
			{
				result = Legendre(a / 2, p);
				if (((p * p - 1) & 8) != 0) result = -result;
			}
			else
			{
				result = Legendre(p % a, a);
				if ((((a - 1) * (p - 1)) & 4) != 0) result = -result;
			}

			return result;
		}

		private static void Swap(ref BigInteger a, ref BigInteger b)
		{
			var c = a;
			a = b;
			b = c;
		}

		public static BigInteger Jacobi(BigInteger a, BigInteger n)
		{
			if (a >= n) a %= n;
			var result = 1;
			while (a != 0)
			{
				while ((a & 1) == 0)
				{
					a >>= 1;
					if ((n & 7) == 3 || (n & 7) == 5) result = -result;
				}

				Swap(ref a, ref n);
				if ((a & 3) == 3 && (n & 3) == 3) result = -result;
				a %= n;
			}

			return n == 1 ? result : 0;
		}

		#endregion
	}
}