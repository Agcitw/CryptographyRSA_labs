using System;
using System.Numerics;

namespace Crypto3
{
	internal static class Program
	{
		public static void Main()
		{
			var r1 = MathTools.EuclidAlgorithm(new BigInteger(210000000000000000), new BigInteger(750000000000000000));
			Console.WriteLine(r1);
			var r2 = MathTools.EuclidAlgorithmExtended(new BigInteger(210000000000000000),
				new BigInteger(750000000000000000));
			Console.WriteLine(r2.X + " " + r2.Y + " " + r2.D);
			var r3 = MathTools.FastExp(new BigInteger(2), new BigInteger(5), new BigInteger(13));
			Console.WriteLine(r3);
			var r4 = MathTools.EulerFunc(new BigInteger(36));
			Console.WriteLine(r4);
			var r5 = MathTools.Legendre(new BigInteger(126), new BigInteger(53));
			Console.WriteLine(r5);
			var r6 = MathTools.Jacobi(new BigInteger(131), new BigInteger(255));
			Console.WriteLine(r6);

			var solStrTest = MathTools.SolovayStrassen.Execute(new BigInteger(19), 2);
			Console.WriteLine(solStrTest);
			var milRabTest = MathTools.MillerRabin.Execute(new BigInteger(19), 2);
			Console.WriteLine(milRabTest);
			var fermatTest = MathTools.Fermat.Execute(new BigInteger(19), 2);
			Console.WriteLine(fermatTest);
		}
	}
}