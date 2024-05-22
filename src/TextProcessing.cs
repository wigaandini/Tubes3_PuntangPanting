using System;
using System.Text.RegularExpressions;
using System.Linq;

public class TextProcessing
{
    public static string NumToChar(string teks)
    {
        return Regex.Replace(teks, @"([^\d]*)([012345789])", match =>
        {
            string[] replacements = { "o", "i", "z", "e", "a", "s", "", "t", "b", "g" };
            return match.Groups[1].Value + replacements[int.Parse(match.Groups[2].Value)];
        });
    }

    public static string AlayNormalization(string teks)
    {
        teks = teks.ToLower();
        teks = NumToChar(teks);
        return teks;
    }

    public static string RemoveVokal(string teks)
    {
        return Regex.Replace(teks, "[aiueo]", "", RegexOptions.IgnoreCase);
    }

    public static bool CompareWord(string sentence, string sourceSentence)
    {
        bool RegexExtractVokalAndCompare(string word, string source)
        {
            var vokalWord = Regex.Matches(word, "[aiueo]", RegexOptions.IgnoreCase).Cast<Match>().Select(m => m.Value).ToArray();
            var vokalSource = Regex.Matches(source, "[aiueo]", RegexOptions.IgnoreCase).Cast<Match>().Select(m => m.Value).ToArray();
            var noVokalWord = Regex.Replace(word, "[aiueo]", "", RegexOptions.IgnoreCase);
            var noVokalSource = Regex.Replace(source, "[aiueo]", "", RegexOptions.IgnoreCase);

            if (noVokalWord != noVokalSource)
                return false;

            if (vokalWord.Length > vokalSource.Length)
            {
                var temp = vokalWord;
                vokalWord = vokalSource;
                vokalSource = temp;
            }

            string pattern = $"[{string.Join("", vokalSource)}]";
            return vokalWord.All(v => Regex.IsMatch(v, pattern));
        }

        var arrayWord = sentence.Split(' ');
        var arraySource = sourceSentence.Split(' ');

        if (arrayWord.Length != arraySource.Length)
            return false;

        return arrayWord.Zip(arraySource, (word, source) => RegexExtractVokalAndCompare(word, source)).All(result => result);
    }

    public static void Main(string[] args)
    {
        // Test the functions here if needed
        string teks = "dn 4dl omo";
        string teks_asli = "dania adalia oemaoe";

        bool comparisonResult = CompareWord(teks, teks_asli);
        Console.WriteLine("Comparison Result: " + comparisonResult);
    }
}
