using System;
using System.Text;

namespace AESExample
{
    public class Sender
    {
        public (byte[] ciphertext, byte[] key, byte[] iv) Encrypt(string plaintext)
        {
            byte[] key = GenerateRandomBytes(32); // 256-bit key
            byte[] iv = GenerateRandomBytes(16);  // 128-bit IV

            CustomAes aes = new CustomAes(key, iv);
            byte[] plaintextBytes = Padding(plaintext);

            byte[] ciphertext = aes.Encrypt(plaintextBytes);

            return (ciphertext, key, iv);
        }

        private byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            new Random().NextBytes(bytes);
            return bytes;
        }

        private byte[] Padding(string input)
        {
            int paddingSize = 16 - (input.Length % 16);
            byte[] paddedInput = new byte[input.Length + paddingSize];
            Array.Copy(Encoding.UTF8.GetBytes(input), paddedInput, input.Length);
            for (int i = input.Length; i < paddedInput.Length; i++)
            {
                paddedInput[i] = (byte)paddingSize;
            }
            return paddedInput;
        }
    }
}

