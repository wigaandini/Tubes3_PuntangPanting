using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test {
    public class KM {
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
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text)) {
                return -1;
            }

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

            return -1;
        }
    }

}
