using System;

namespace TestProgram {
    public class BMAlgo {
        private static int[] LastOccurrence(string pattern) {
            int[] last = new int[128];

            for (int i = 0; i < 128; i++) {
                last[i] = -1;
            }

            for (int i = 0; i < pattern.Length; i++) {
                last[pattern[i]] = i;
            }

            return last;
        }

        public static int Match(string pattern, string text) {
            int[] last = LastOccurrence(pattern);
            int n = text.Length;
            int m = pattern.Length;
            int i = m - 1;

            if (i > n - 1) {
                return -1;
            } else {
                int j = m - 1;
                while (true) {
                    if (pattern[j] == text[i]) {
                        if (j == 0) {
                            return i;
                        } else {
                            i--;
                            j--;
                        }
                    } else {
                        int lo = last[text[i]];
                        i += m - Math.Min(j, 1 + lo);
                        j = m - 1;
                    }

                    if (i > n - 1) {
                        break;
                    }
                }
            }

            // No exact match found, use Levenshtein
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

            // No exact match found, calculate Levenshtein Distance
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