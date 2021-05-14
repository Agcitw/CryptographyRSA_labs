using System.Numerics;
using Crypto3;
using Xunit;

namespace WienerAttack
{
	public class Tests
	{
		private static void WienerAttackTest(RSA rsa, BigInteger openText)
		{
			var cipherText = rsa.Encrypt(openText);
			Assert.Equal(openText, RSA.WienerAttack(cipherText, rsa.PublicKey, rsa.Mod));
		}

		[Fact]
		public void WienerAttack_1()
		{
			var rsa = new RSA(1, 1) {PublicKey = 1};
			WienerAttackTest(rsa, 112);
			WienerAttackTest(rsa, 197);
			WienerAttackTest(rsa, 512);
			WienerAttackTest(rsa, 1);
			WienerAttackTest(rsa, 5700);
		}

		[Fact]
		public void WienerAttack_2()
		{
			var rsa = new RSA(199, 197) {PublicKey = 5};
			WienerAttackTest(rsa, 112);
			WienerAttackTest(rsa, 30000);
			WienerAttackTest(rsa, 12345);
			WienerAttackTest(rsa, 39123);
			WienerAttackTest(rsa, 1);
		}

		[Fact]
		public void WienerAttack_3()
		{
			var rsa = new RSA(1000000007, 1000000009) {PublicKey = 29};
			WienerAttackTest(rsa, 112);
			WienerAttackTest(rsa, 30000);
			WienerAttackTest(rsa, 12345);
			WienerAttackTest(rsa, 39123);
			WienerAttackTest(rsa, 1);
		}

		[Fact]
		public void WienerAttack_4()
		{
			var rsa = new RSA(59, 160001) {PublicKey = 7};
			WienerAttackTest(rsa, 1145275274572);
			WienerAttackTest(rsa, 30000457425);
			WienerAttackTest(rsa, 12347428755);
			WienerAttackTest(rsa, 3912457452723);
			WienerAttackTest(rsa, 1457452724522);
		}
	}
}