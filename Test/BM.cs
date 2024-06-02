using System;
using System.Collections.Generic;

namespace Test {
    public class BM {
        private static Dictionary<char, int> LastOccurrence(string pattern) {
            var last = new Dictionary<char, int>();

            for (int i = 0; i < pattern.Length; i++) {
                last[pattern[i]] = i;
            }

            return last;
        }

        public static int Match(string pattern, string text) {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text)) {
                return -1;
            }

            var last = LastOccurrence(pattern);
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
                        int lo = last.ContainsKey(text[i]) ? last[text[i]] : -1;
                        i += m - Math.Min(j, 1 + lo);
                        j = m - 1;
                    }

                    if (i > n - 1) {
                        break;
                    }
                }
            }

            return -1;
        }

    }
}
