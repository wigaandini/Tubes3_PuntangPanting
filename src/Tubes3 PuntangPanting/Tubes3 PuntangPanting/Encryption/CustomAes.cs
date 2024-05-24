using System;

namespace AESExample
{
    public class CustomAes
    {
        private const int BlockSize = 16; // AES block size is 16 bytes (128 bits)
        private const int KeySize = 16; // AES-128 uses 16-byte (128-bit) key
        private const int Nr = 10; // Number of rounds for AES-128
        private const int Nk = 4; // Number of 32-bit words in the key for AES-128
        private const int Nb = 4; // Number of columns (32-bit words) in the state

        private readonly byte[] SBox = new byte[256]{
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };

        private readonly byte[] InvSBox = new byte[256]{
            0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
            0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
            0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
            0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
            0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
            0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
            0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
            0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
            0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
            0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
            0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
            0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
            0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
            0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
            0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };

        private static readonly byte[] Rcon = new byte[256] {
            0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public CustomAes(byte[] key, byte[] iv)
        {
            if (key.Length != KeySize)
                throw new ArgumentException("Key size must be 128 bits.");

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
            byte[] xorBlock = new byte[BlockSize];
            Array.Copy(IV, xorBlock, BlockSize);

            for (int i = 0; i < plaintext.Length; i += BlockSize)
            {
                byte[] block = new byte[BlockSize];
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
            byte[] xorBlock = new byte[BlockSize];
            Array.Copy(IV, xorBlock, BlockSize);

            for (int i = 0; i < ciphertext.Length; i += BlockSize)
            {
                byte[] block = new byte[BlockSize];
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
            byte[,] state = new byte[4, 4];
            for (int i = 0; i < BlockSize; i++)
            {
                state[i % 4, i / 4] = block[i];
            }

            byte[,] roundKeys = KeyExpansion(Key);

            AddRoundKey(state, roundKeys, 0);

            for (int round = 1; round < Nr; round++)
            {
                SubBytes(state);
                ShiftRows(state);
                MixColumns(state);
                AddRoundKey(state, roundKeys, round);
            }

            SubBytes(state);
            ShiftRows(state);
            AddRoundKey(state, roundKeys, Nr);

            byte[] encryptedBlock = new byte[BlockSize];
            for (int i = 0; i < BlockSize; i++)
            {
                encryptedBlock[i] = state[i % 4, i / 4];
            }

            return encryptedBlock;
        }

        private byte[] DecryptBlock(byte[] block)
        {
            byte[,] state = new byte[4, 4];
            for (int i = 0; i < BlockSize; i++)
            {
                state[i % 4, i / 4] = block[i];
            }

            byte[,] roundKeys = KeyExpansion(Key);

            AddRoundKey(state, roundKeys, Nr);

            for (int round = Nr - 1; round >= 1; round--)
            {
                InvShiftRows(state);
                InvSubBytes(state);
                AddRoundKey(state, roundKeys, round);
                InvMixColumns(state);
            }

            InvShiftRows(state);
            InvSubBytes(state);
            AddRoundKey(state, roundKeys, 0);

            byte[] decryptedBlock = new byte[BlockSize];
            for (int i = 0; i < BlockSize; i++)
            {
                decryptedBlock[i] = state[i % 4, i / 4];
            }

            return decryptedBlock;
        }

        private byte[,] KeyExpansion(byte[] key)
        {
            byte[,] roundKeys = new byte[Nb * (Nr + 1), 4];

            for (int i = 0; i < Nk; i++)
            {
                roundKeys[i, 0] = key[4 * i];
                roundKeys[i, 1] = key[4 * i + 1];
                roundKeys[i, 2] = key[4 * i + 2];
                roundKeys[i, 3] = key[4 * i + 3];
            }

            for (int i = Nk; i < Nb * (Nr + 1); i++)
            {
                byte[] temp = { roundKeys[i - 1, 0], roundKeys[i - 1, 1], roundKeys[i - 1, 2], roundKeys[i - 1, 3] };

                if (i % Nk == 0)
                {
                    temp = SubWord(RotWord(temp));
                    temp[0] ^= Rcon[i / Nk];
                }

                for (int j = 0; j < 4; j++)
                {
                    roundKeys[i, j] = (byte)(roundKeys[i - Nk, j] ^ temp[j]);
                }
            }

            return roundKeys;
        }

        private byte[] SubWord(byte[] word)
        {
            for (int i = 0; i < 4; i++)
            {
                word[i] = SBox[word[i]];
            }
            return word;
        }

        private byte[] RotWord(byte[] word)
        {
            byte temp = word[0];
            word[0] = word[1];
            word[1] = word[2];
            word[2] = word[3];
            word[3] = temp;
            return word;
        }

        private void AddRoundKey(byte[,] state, byte[,] roundKeys, int round)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    state[j, i] ^= roundKeys[round * Nb + i, j];
                }
            }
        }

        private void SubBytes(byte[,] state)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    state[i, j] = SBox[state[i, j]];
                }
            }
        }

        private void ShiftRows(byte[,] state)
        {
            for (int i = 1; i < 4; i++)
            {
                byte[] row = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    row[j] = state[i, (j + i) % Nb];
                }
                for (int j = 0; j < 4; j++)
                {
                    state[i, j] = row[j];
                }
            }
        }

        private void MixColumns(byte[,] state)
        {
            for (int i = 0; i < 4; i++)
            {
                byte[] col = { state[0, i], state[1, i], state[2, i], state[3, i] };
                state[0, i] = (byte)(GaloisMul(col[0], 2) ^ GaloisMul(col[1], 3) ^ col[2] ^ col[3]);
                state[1, i] = (byte)(col[0] ^ GaloisMul(col[1], 2) ^ GaloisMul(col[2], 3) ^ col[3]);
                state[2, i] = (byte)(col[0] ^ col[1] ^ GaloisMul(col[2], 2) ^ GaloisMul(col[3], 3));
                state[3, i] = (byte)(GaloisMul(col[0], 3) ^ col[1] ^ col[2] ^ GaloisMul(col[3], 2));
            }
        }

        private void InvSubBytes(byte[,] state)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    state[i, j] = InvSBox[state[i, j]];
                }
            }
        }

        private void InvShiftRows(byte[,] state)
        {
            for (int i = 1; i < 4; i++)
            {
                byte[] row = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    row[(j + i) % Nb] = state[i, j];
                }
                for (int j = 0; j < 4; j++)
                {
                    state[i, j] = row[j];
                }
            }
        }

        private void InvMixColumns(byte[,] state)
        {
            for (int i = 0; i < 4; i++)
            {
                byte[] col = { state[0, i], state[1, i], state[2, i], state[3, i] };
                state[0, i] = (byte)(GaloisMul(col[0], 14) ^ GaloisMul(col[1], 11) ^ GaloisMul(col[2], 13) ^ GaloisMul(col[3], 9));
                state[1, i] = (byte)(GaloisMul(col[0], 9) ^ GaloisMul(col[1], 14) ^ GaloisMul(col[2], 11) ^ GaloisMul(col[3], 13));
                state[2, i] = (byte)(GaloisMul(col[0], 13) ^ GaloisMul(col[1], 9) ^ GaloisMul(col[2], 14) ^ GaloisMul(col[3], 11));
                state[3, i] = (byte)(GaloisMul(col[0], 11) ^ GaloisMul(col[1], 13) ^ GaloisMul(col[2], 9) ^ GaloisMul(col[3], 14));
            }
        }

        private byte GaloisMul(byte a, byte b)
        {
            byte p = 0;
            byte hiBitSet;
            for (int counter = 0; counter < 8; counter++)
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }
                hiBitSet = (byte)(a & 0x80);
                a <<= 1;
                if (hiBitSet != 0)
                {
                    a ^= 0x1b; // x^8 + x^4 + x^3 + x + 1
                }
                b >>= 1;
            }
            return p;
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