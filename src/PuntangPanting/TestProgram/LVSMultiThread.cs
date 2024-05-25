using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace TestProgram
{
    public class LVSMultiThread
    {
        private static readonly object _lock = new object();

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        public static double CalculateSimilarityPercentage(string s, string t)
        {
            int levDistance = LevenshteinDistance(s, t);
            int maxLength = Math.Max(s.Length, t.Length);
            return maxLength == 0 ? 100.0 : (1.0 - (double)levDistance / maxLength) * 100.0;
        }

        public static (int textIndex, int index, double similarity) MatchWithLevenshtein(string pattern, List<string> texts, double minPercentage, int algorithm)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return (-1, -1, 0.0);
            }

            double highestSimilarity = 0.0;
            int closestMatchIndex = -1;
            int closestTextIndex = -1;

            Parallel.ForEach(texts, (text, state, index) =>
            {
                if (highestSimilarity >= 100.0) return; // Stop if 100% similarity found

                int matchIndex = algorithm == 1 ? BMAlgo.Match(pattern, text) : KMPAlgo.Match(pattern, text);

                if (matchIndex != -1)
                {
                    lock (_lock)
                    {
                        if (highestSimilarity < 100.0)
                        {
                            closestTextIndex = (int)index;
                            closestMatchIndex = matchIndex;
                            highestSimilarity = 100.0;
                        }
                    }
                    return;
                }

                for (int i = 0; i <= text.Length - pattern.Length; i++)
                {
                    string substring = text.Substring(i, pattern.Length);
                    double similarity = CalculateSimilarityPercentage(pattern, substring);

                    if (similarity > highestSimilarity)
                    {
                        lock (_lock)
                        {
                            if (similarity > highestSimilarity)
                            {
                                highestSimilarity = similarity;
                                closestMatchIndex = i;
                                closestTextIndex = (int)index;
                            }
                        }
                    }
                }
            });

            if (highestSimilarity >= minPercentage)
            {
                return (closestTextIndex, closestMatchIndex, highestSimilarity);
            }
            else
            {
                return (-1, -1, highestSimilarity);
            }
        }
    }
}
