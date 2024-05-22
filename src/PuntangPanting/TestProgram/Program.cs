using System;
using System.Diagnostics;

namespace TestProgram {

    class Program {
        static void Main(string[] args) {
            string text = "ï:åñÏÞùàYæ??ß3Ós|¼ãøÆ:Þî3Ùâ¹Æ39ÏãÛíÆ0½ÿ;ÏûáÆg¸>3ÂðÌ|3Ìfüæ0øf1 38ÿ}8yÇÞdþnsÞ39è9<?ÙíÌøÄF";
            string pattern = "Æ7ÞäaóÖg1ç|þÜ`[b#3I¦|H$Áe²+l%òÓ)¶KL¤Û)I$ $ÌRfIi{#f­¤Éµ";
            double minPercentage = 70.0;

            Stopwatch stopwatch = new Stopwatch();

            var resultBM = BMAlgo.MatchWithLevenshtein(pattern, text, minPercentage);

            if (resultBM.index != -1) {
                Console.WriteLine($"Pattern found at index {resultBM.index} with similarity {resultBM.similarity}%");
            } else {
                Console.WriteLine($"No match found with minimum similarity of {minPercentage}%. Highest similarity found was {resultBM.similarity}%.");
            }

            Console.WriteLine("BM");
            Console.WriteLine($"Index: {resultBM.index}, Similarity: {resultBM.similarity}%");
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}

    // class Program {
        // static void Main(string[] args) {
        //     string text = "ï:åñÏÞùàYæ??ß3Ós|¼ãøÆ:Þî3Ùâ¹Æ39ÏãÛíÆ0½ÿ;ÏûáÆg¸>3ÂðÌ|3Ìfüæ0øf1 38ÿ}8yÇÞdþnsÞ39è9<?ÙíÌøÄF";
        //     string pattern = "Æ7ÞäaóÖg1ç|þÜ`[b#3I¦|H$Áe²+l%òÓ)¶KL¤Û)I$ $ÌRfIi{#f­¤Éµ";
        //     // string pattern = "ï:åñÏÞùàYæ??ß3Ós|¼ãøÆ:Þî3Ùâ¹Æ39ÏãÛíÆ0½ÿ;ÏûáÆg¸>3ÂðÌ|3Ìfüæ0øf1 38ÿ}8yÇÞdþnsÞ39è9<?ÙíÌøÄF";

        //     Stopwatch stopwatch = new Stopwatch();

        //     stopwatch.Start();
        //     var resultKMP = KMPAlgo.MatchWithLevenshtein(pattern, text, 70.0);
        //     // var resultBM = BMAlgo.MatchWithLevenshtein(pattern, text, 70.0);
        //     stopwatch.Stop();

        //     Console.WriteLine("KMP");
        //     Console.WriteLine($"Index: {resultKMP.index}, Similarity: {resultKMP.similarity}%");
        //     Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            
        // }
        
    // }