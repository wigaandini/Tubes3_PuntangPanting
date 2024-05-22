using System;

namespace AESExample
{
    public class CustomAes
    {
        private const int BlockSize = 16; // AES block size is 16 bytes (128 bits)

        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public CustomAes(byte[] key, byte[] iv)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                throw new ArgumentException("Key size must be 128, 192, or 256 bits.");

            if (iv.Length != BlockSize)
                throw new ArgumentException("IV size must be 128 bits.");

            Key = key;
            IV = iv;
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            if (plaintext.Length % BlockSize != 0)
                throw new ArgumentException("Plaintext size must be a multiple of 128 bits.");

            byte[] ciphertext = new byte[plaintext.Length];
            byte[] block = new byte[BlockSize];
            byte[] xorBlock = new byte[BlockSize];
            Array.Copy(IV, xorBlock, BlockSize);

            for (int i = 0; i < plaintext.Length; i += BlockSize)
            {
                Array.Copy(plaintext, i, block, 0, BlockSize);
                XorBlocks(block, xorBlock, block);
                byte[] encryptedBlock = EncryptBlock(block);
                Array.Copy(encryptedBlock, 0, ciphertext, i, BlockSize);
                Array.Copy(encryptedBlock, xorBlock, BlockSize);
            }

            return ciphertext;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            if (ciphertext.Length % BlockSize != 0)
                throw new ArgumentException("Ciphertext size must be a multiple of 128 bits.");

            byte[] plaintext = new byte[ciphertext.Length];
            byte[] block = new byte[BlockSize];
            byte[] xorBlock = new byte[BlockSize];
            Array.Copy(IV, xorBlock, BlockSize);

            for (int i = 0; i < ciphertext.Length; i += BlockSize)
            {
                Array.Copy(ciphertext, i, block, 0, BlockSize);
                byte[] decryptedBlock = DecryptBlock(block);
                XorBlocks(decryptedBlock, xorBlock, decryptedBlock);
                Array.Copy(decryptedBlock, 0, plaintext, i, BlockSize);
                Array.Copy(block, xorBlock, BlockSize);
            }

            return plaintext;
        }

        private byte[] EncryptBlock(byte[] block)
        {
            // Simplified: Actual AES encryption rounds are complex and involve multiple steps
            // Here we just copy the block for demonstration purposes
            byte[] encryptedBlock = new byte[BlockSize];
            Array.Copy(block, encryptedBlock, BlockSize);
            return encryptedBlock;
        }

        private byte[] DecryptBlock(byte[] block)
        {
            // Simplified: Actual AES decryption rounds are complex and involve multiple steps
            // Here we just copy the block for demonstration purposes
            byte[] decryptedBlock = new byte[BlockSize];
            Array.Copy(block, decryptedBlock, BlockSize);
            return decryptedBlock;
        }

        private void XorBlocks(byte[] block1, byte[] block2, byte[] output)
        {
            for (int i = 0; i < BlockSize; i++)
            {
                output[i] = (byte)(block1[i] ^ block2[i]);
            }
        }
    }
}
