using System;
using System.Collections.Generic;

namespace TestProgram {
    public class Levenshtein {
        public static int LevenshteinDistance(string s, string t) {
            if (string.IsNullOrEmpty(s)) {
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            }

            if (string.IsNullOrEmpty(t)) {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++) {
                d[0, j] = j;
            }

            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        public static double CalculateSimilarityPercentage(string s, string t) {
            int levDistance = LevenshteinDistance(s, t);
            int maxLength = Math.Max(s.Length, t.Length);
            return maxLength == 0 ? 100.0 : (1.0 - (double)levDistance / maxLength) * 100.0;
        }

        public static (int index, double similarity) MatchWithLevenshtein(string pattern, List<string> texts, double minPercentage, int algo) {

            int exactMatchIndex = -1;
            foreach (var text in texts) {
                if (pattern.Length == 0) {
                    if (text.Length == 0) {
                        return (0, 100.0);
                    }
                    else {
                        return (-1, 0.0); 
                    }
                }
                if (algo == 1) { // BM
                    exactMatchIndex = BMAlgo.Match(pattern, new List<string> { text });
                } else if (algo == 2) { // KMP
                    exactMatchIndex = KMPAlgo.Match(pattern, new List<string> { text });
                }
                if (exactMatchIndex != -1) {
                    return (exactMatchIndex, 100.0);
                }
            }

            double highestSimilarity = 0.0;
            int closestMatchIndex = -1;

            foreach (var text in texts) {
                for (int i = 0; i <= text.Length - pattern.Length; i++) {
                    string substring = text.Substring(i, pattern.Length);
                    double similarity = CalculateSimilarityPercentage(pattern, substring);

                    if (similarity > highestSimilarity) {
                        highestSimilarity = similarity;
                        closestMatchIndex = i;
                    }
                }
            }

            if (highestSimilarity >= minPercentage) {
                return (closestMatchIndex, highestSimilarity);
            } else {
                return (-1, highestSimilarity);
            }
        }

    }
}