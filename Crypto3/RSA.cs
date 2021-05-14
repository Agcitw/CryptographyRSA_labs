using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Crypto3
{
	// ReSharper disable once InconsistentNaming
	public class RSA
	{
		private BigInteger _publicKey;

        public BigInteger PublicKey
        {
            get => _publicKey;
            set
            {
                if (CheckKey(value) == false)
                    throw new ArgumentException();
                _publicKey = value;
                PrivateKey = CalcSecondKey(_publicKey);
            }
        }

        private BigInteger PrivateKey { get; set; }

        public readonly BigInteger Mod;
        private readonly BigInteger _order;

        public RSA(BigInteger firstModPart, BigInteger secondModPart)
        {
            if (!CheckModPart(firstModPart) || !CheckModPart(secondModPart))
                throw new ArgumentException();
            Mod = firstModPart * secondModPart;
            _order = (firstModPart - 1) * (secondModPart - 1);
            PublicKey = 1;
        }

        private bool CheckKey(BigInteger publicKey)
        {
            return Gcd(publicKey, _order) == 1;
        }

        private static BigInteger CalcSecondKey(BigInteger firstKey, BigInteger order)
        {
            Gcd(firstKey, order, out var u, out _);
            u %= order;
            while (u < 0)
                u += order;
            return u;
        }

        private BigInteger CalcSecondKey(BigInteger firstKey)
        {
            return CalcSecondKey(firstKey, _order);
        }

        private static bool CheckModPart(BigInteger modPart)
        {
            if (modPart == 1)
                return false;
            for (BigInteger i = 2; i * i <= modPart; i++)
            {
                if (modPart % i == 0)
                    return false;
            }
            return true;
        }

        private static BigInteger Encrypt(BigInteger openText, BigInteger mod, BigInteger publicKey)
        {
            return BinPow(openText, publicKey, mod);
        }

        public BigInteger Encrypt(BigInteger openText)
        {
            return Encrypt(openText, Mod, PublicKey);
        }

        private static BigInteger Decrypt(BigInteger cipherText, BigInteger mod, BigInteger privateKey)
        {
            return BinPow(cipherText, privateKey, mod);
        }

        public BigInteger Decrypt(BigInteger cipherText)
        {
            return Decrypt(cipherText, Mod, PrivateKey);
        }

        private static BigInteger BinPow(BigInteger a, BigInteger x, BigInteger mod)
        {
            var res = new BigInteger(1);
            a %= mod;
            while (x != 0)
            {
                if ((x & 1) == 1)
                {
                    res *= a;
                    res %= mod;
                }

                x >>= 1;
                a = (a * a) % mod;
            }
            return res;
        }

        private static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                a %= b;
                Swap(ref a, ref b);
            }
            return a;
        }

        private static void Gcd(BigInteger a, BigInteger b, out BigInteger u, out BigInteger v)
        {
            var x = new BigInteger[2, 2];
            x[0, 0] = 1;
            x[0, 1] = 0;
            x[1, 0] = 0;
            x[1, 1] = 1;
            while (b != 0)
            {
                var y = new BigInteger[2, 2];
                var q = a / b;
                var r = a - b * q;
                y[0, 0] = x[0, 1];
                y[0, 1] = x[0, 0] - q * x[0, 1];
                y[1, 0] = x[1, 1];
                y[1, 1] = x[1, 0] - q * x[1, 1];
                a = b;
                b = r;
                x = y;
            }
            u = x[0, 0];
            v = x[1, 0];
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            var c = a;
            a = b;
            b = c;
        }

        public static BigInteger WienerAttack(BigInteger cipherText, BigInteger publicKey,
            BigInteger mod)
        {
            var continuedFraction = GetContinuedFraction(publicKey, mod).ToList();
            var convergents = GetConvergents(continuedFraction).ToList();
            var privateKey = GetPrivateKey(convergents, publicKey, mod);
            return Decrypt(cipherText, mod, privateKey);
        }

        private static IEnumerable<BigInteger> GetContinuedFraction(BigInteger numerator, BigInteger denominator)
        {
            if(denominator == 0)
                throw new DivideByZeroException();
            var gcd = Gcd(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            while (denominator != 0)
            {
                var a = numerator / denominator;
                yield return a;
                numerator -= a * denominator;
                Swap(ref numerator, ref denominator);
            }
        }

        private static IEnumerable<(BigInteger,BigInteger)> GetConvergents(IEnumerable<BigInteger> continuedFractions)
        {
            var previousConvergents = new (BigInteger, BigInteger)[2];
            previousConvergents[0] = (new BigInteger(0), new BigInteger(1));
            previousConvergents[1] = (new BigInteger(1), new BigInteger(0));
            foreach (var a in continuedFractions)
            {
                var p = a * previousConvergents[1].Item1 + previousConvergents[0].Item1;
                var q = a * previousConvergents[1].Item2 + previousConvergents[0].Item2;
                yield return (p, q);
                previousConvergents[0] = previousConvergents[1];
                previousConvergents[1] = (p, q);
            }
        }

        private static BigInteger GetPrivateKey(IEnumerable<(BigInteger, BigInteger)> convergents, BigInteger publicKey,
            BigInteger mod)
        {
            foreach (var (p, q) in convergents)
                if (CheckConvergent((p, q), publicKey, mod))
                    return q;
            throw new Exception("Wiener's attack failed");
        }

        private static bool CheckConvergent((BigInteger, BigInteger) convergent, BigInteger publicKey, BigInteger mod)
        {
            var (p, q) = convergent;
            if (p == 0)
                return false;
            var phi = (publicKey * q - 1) / p;
            var bCoef = mod - phi + 1;
            var discriminant = bCoef * bCoef - 4 * mod;
            var sqrtDiscriminant = LowerSqrt(discriminant);
            if (sqrtDiscriminant * sqrtDiscriminant != discriminant)
                return false;
            var x1 = (bCoef + sqrtDiscriminant) >> 1;
            var x2 = x1 - sqrtDiscriminant;
            return x1 * x2 == mod;
        }

        private static BigInteger LowerSqrt(BigInteger n)
        {
            BigInteger l = 1, r = n;
            while (r - l > 1)
            {
                var m = (r + l) >> 1;
                if (m * m <= n)
                    l = m;
                else
                    r = m;
            }
            return l;
        }
	}
}