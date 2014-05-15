using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace MatchMaker
{

	public class PasswordEncryptor : IPasswordEncryptor
	{

		private readonly static byte[] IV = new byte[] { 248, 169, 13, 123, 240, 15, 162, 10 };
		private readonly static byte[] Key = new byte[] { 232, 65, 109, 50, 27, 228, 176, 96 };


		private readonly DESCryptoServiceProvider des = new DESCryptoServiceProvider();


		public string Encrypt( string plainTextPassword )
		{
			var unencryptedBytes = Encoding.Unicode.GetBytes(plainTextPassword);

			using (var outputStream = new MemoryStream())
			{
				using (var encryptionStream = new CryptoStream(outputStream, des.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
				{
					encryptionStream.Write(unencryptedBytes, 0, unencryptedBytes.Length);
				}

				var encryptedBytes = outputStream.ToArray();

				var encryptedString = Convert.ToBase64String(encryptedBytes);

				return encryptedString;
			}
		}

	}
}