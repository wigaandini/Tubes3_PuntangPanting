namespace AESExample
{
    public static class Utf8Decoder
    {
        public static string Decode(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            int charIndex = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                byte currentByte = bytes[i];

                if ((currentByte & 0x80) == 0) // 1-byte character
                {
                    chars[charIndex++] = (char)currentByte;
                }
                else if ((currentByte & 0xE0) == 0xC0) // 2-byte character
                {
                    if (i + 1 >= bytes.Length) throw new ArgumentException("Invalid UTF-8 sequence");
                    char unicodeChar = (char)(((currentByte & 0x1F) << 6) | (bytes[++i] & 0x3F));
                    chars[charIndex++] = unicodeChar;
                }
                else if ((currentByte & 0xF0) == 0xE0) // 3-byte character
                {
                    if (i + 2 >= bytes.Length) throw new ArgumentException("Invalid UTF-8 sequence");
                    char unicodeChar = (char)(((currentByte & 0x0F) << 12) | ((bytes[++i] & 0x3F) << 6) | (bytes[++i] & 0x3F));
                    chars[charIndex++] = unicodeChar;
                }
                else if ((currentByte & 0xF8) == 0xF0) // 4-byte character
                {
                    if (i + 3 >= bytes.Length) throw new ArgumentException("Invalid UTF-8 sequence");
                    int unicodeChar = ((currentByte & 0x07) << 18) | ((bytes[++i] & 0x3F) << 12) | ((bytes[++i] & 0x3F) << 6) | (bytes[++i] & 0x3F);
                    if (unicodeChar > 0xFFFF)
                    {
                        unicodeChar -= 0x10000;
                        chars[charIndex++] = (char)((unicodeChar >> 10) + 0xD800);
                        chars[charIndex++] = (char)((unicodeChar & 0x3FF) + 0xDC00);
                    }
                    else
                    {
                        chars[charIndex++] = (char)unicodeChar;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid UTF-8 byte");
                }
            }

            return new string(chars, 0, charIndex);
        }
    }
}
