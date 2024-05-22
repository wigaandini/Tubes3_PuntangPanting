using System;

namespace TestProgram {
    public class KMPAlgo {
        private static int[] Border(string pattern) {
            int[] b = new int[pattern.Length];
            b[0] = 0;

            int m = pattern.Length;
            int j = 0;
            int i = 1;

            while (i < m) {
                if (pattern[j] == pattern[i]) {
                    b[i] = j + 1;
                    i++;
                    j++;
                } else if (j > 0) {
                    j = b[j - 1];
                } else {
                    b[i] = 0;
                    i++;
                }
            }
            return b;
        }

        public static int Match(string pattern, string text) {
            int n = text.Length;
            int m = pattern.Length;
            int[] b = Border(pattern);

            int i = 0;
            int j = 0;

            while (i < n) {
                if (pattern[j] == text[i]) {
                    if (j == m - 1) {
                        return i - m + 1;
                    }
                    i++;
                    j++;
                } else if (j > 0) {
                    j = b[j - 1];
                } else {
                    i++;
                }
            }

            // Kalau gada yg match, cari pake Levenshtein
            return -1;
        }

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

        public static (int index, double similarity) MatchWithLevenshtein(string pattern, string text, double minPercentage) {
            int exactMatchIndex = Match(pattern, text);
            if (exactMatchIndex != -1) {
                return (exactMatchIndex, 100.0);
            }

            // Cari Levenshtein distance
            double highestSimilarity = 0.0;
            int closestMatchIndex = -1;

            for (int i = 0; i <= text.Length - pattern.Length; i++) {
                string substring = text.Substring(i, pattern.Length);
                double similarity = CalculateSimilarityPercentage(pattern, substring);

                if (similarity > highestSimilarity) {
                    highestSimilarity = similarity;
                    closestMatchIndex = i;
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
