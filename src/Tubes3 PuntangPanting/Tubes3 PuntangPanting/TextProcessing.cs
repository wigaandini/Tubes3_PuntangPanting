using System;
using System.Text.RegularExpressions;

namespace Tubes3_PuntangPanting
{
    class TextProcessing
    {
        static string NumToChar(string teks)
        {
            return Regex.Replace(teks, @"([^\d]*)([012345789])", match =>
            {
                string group1 = match.Groups[1].Value;
                char group2 = match.Groups[2].Value[0];
                string replacement = group2 switch
                {
                    '1' => "i",
                    '2' => "z",
                    '3' => "e",
                    '4' => "a",
                    '5' => "s",
                    '7' => "t",
                    '8' => "b",
                    '9' => "g",
                    '0' => "o",
                    _ => group2.ToString(),
                };
                return group1 + replacement;
            });
        }

        static string AlayNormalization(string teks)
        {
            teks = teks.ToLower();
            teks = NumToChar(teks);
            return teks;
        }

        static string RemoveVokal(string teks)
        {
            return Regex.Replace(teks, "[aiueo]", "");
        }

        static bool CompareWord(string sentence, string sourceSentence)
        {
            bool RegexExtractVokalAndCompare(string word, string source)
            {
                var vokalWord = Regex.Matches(word, "[aiueo]");
                var vokalSource = Regex.Matches(source, "[aiueo]");
                string noVokalWord = RemoveVokal(word);
                string noVokalSource = RemoveVokal(source);

                if (noVokalWord != noVokalSource)
                    return false;

                var longer = vokalWord.Count > vokalSource.Count ? vokalWord : vokalSource;
                var shorter = vokalWord.Count > vokalSource.Count ? vokalSource : vokalWord;

                string pattern = $"[{string.Join("", longer)}]";
                foreach (Match match in shorter)
                {
                    if (!Regex.IsMatch(match.Value, pattern))
                        return false;
                }
                return true;
            }

            string[] arrayWord = sentence.Split(' ');
            string[] arraySource = sourceSentence.Split(' ');

            if (arrayWord.Length != arraySource.Length)
                return false;

            for (int i = 0; i < arrayWord.Length; i++)
            {
                if (!RegexExtractVokalAndCompare(arrayWord[i], arraySource[i]))
                    return false;
            }
            return true;
        }
    }
}