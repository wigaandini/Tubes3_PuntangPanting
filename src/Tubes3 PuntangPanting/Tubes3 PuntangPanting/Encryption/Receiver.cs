using System;

namespace AESExample
{
    public class Receiver
    {
        public string Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            CustomAes aes = new CustomAes(key, iv);
            byte[] decryptedBytes = aes.Decrypt(ciphertext);
            byte[] unpaddedBytes = RemovePadding(decryptedBytes);

            return Utf8Decoder.Decode(unpaddedBytes);
        }

        private byte[] RemovePadding(byte[] input)
        {
            int paddingSize = input[input.Length - 1];
            byte[] unpaddedInput = new byte[input.Length - paddingSize];
            Array.Copy(input, unpaddedInput, unpaddedInput.Length);
            return unpaddedInput;
        }
    }
}
