using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestProgram {
    class Program {
        static void Main(string[] args) {
            List<string> text = new List<string> {
                "ï:åñÏÞùàYæ??ß3Ós|¼ãøÆ:Þî3Ùâ¹Æ39ÏãÛíÆ0½ÿ;ÏûáÆg¸>3ÂðÌ|3Ìfüæ0øf1 38ÿ}8yÇÞdþnsÞ39è9<?ÙíÌøÄF", 
                "ÿ0À<La?ã7 G#pxFÉÀ\"OÄd@pÌ?ð92p>dìÈðdb.o¿", 
                "ãÿç8ø?1 áÿìîÿæ1s£ÏÿøÌx", 
                "9áÿ¹<zã8üqDã@ ñÄ'ùüb â1ùüÇ", 
                "ÿpãÜýsÇ¼üëÇþ=ÃÇÜSÏ?", 
                "Oç;ïÏûÇóÿþþ÷ýÿ8púïxøó¿;øóÎ¼~û®p?{ÿ¼÷æxïÙÏ;Ãñæyå÷wòû¼ñÿç¼ðóó;ø{ÆxùýÎqpó¼yøóüyÿ88þ",
                "Æ7ÞäaóÖg1ç|þÜ`[b#3I¦|H$Áe²+l%òÓ)¶KL¤Û)I$ $ÌRfIi{#f­¤É"
            };
            // string pattern = "æxïÙÏ;Ãñæyå÷wòû¼ñÿç¼ðóó;ø{ÆxùýÎdsó¼yøóüyÿ88þ";
            // string pattern= "¼üëÇþ=ÃaÜSÏ?";
            string pattern = "";
            double minPercentage = 70.0;

            // Boyer-Moore Algorithm
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultBM = Levenshtein.MatchWithLevenshtein(pattern, text, minPercentage, 1); // 1 for BM
            stopwatch.Stop();

            if (resultBM.index != -1) {
                Console.WriteLine($"BM: Pattern found in string at index {resultBM.textIndex} with similarity {resultBM.similarity}% at position {resultBM.index}.");
            } else {
                Console.WriteLine($"BM: No match found with minimum similarity of {minPercentage}%. Highest similarity found was {resultBM.similarity}%.");
            }

            Console.WriteLine($"BM Execution Time: {stopwatch.ElapsedMilliseconds} ms");

            // Knuth-Morris-Pratt Algorithm
            stopwatch.Restart();
            var resultKMP = Levenshtein.MatchWithLevenshtein(pattern, text, minPercentage, 2); // 2 for KMP
            stopwatch.Stop();

            if (resultKMP.index != -1) {
                Console.WriteLine($"KMP: Pattern found in string at index {resultKMP.textIndex} with similarity {resultKMP.similarity}% at position {resultKMP.index}.");
            } else {
                Console.WriteLine($"KMP: No match found with minimum similarity of {minPercentage}%. Highest similarity found was {resultKMP.similarity}%.");
            }

            Console.WriteLine($"KMP Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}