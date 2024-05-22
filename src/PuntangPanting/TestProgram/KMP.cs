using System;
using System.Collections.Generic;

namespace TestProgram {
    public class KMPAlgo {
        private static int[] Border(string pattern) {
            if (String.IsNullOrEmpty(pattern)) {
                return new int[0];
            }

            int[] b = new int[pattern.Length];
            b[0] = 0;

            int m = pattern.Length;
            int j = 0;
            int i = 1;

            while (i < m) {
                if (pattern[i] == pattern[j]) {
                    j++;
                    b[i] = j;
                    i++;
                } else {
                    if (j != 0) {
                        j = b[j - 1];
                    } else {
                        b[i] = 0;
                        i++;
                    }
                }
            }
            return b;
        }

        public static int Match(string pattern, List<string> texts) {
            if (texts.Count == 0) {
                return -1;
            }

            int m = pattern.Length;
            int[] b = Border(pattern);

            foreach (string text in texts) {
                if (pattern.Length == 0 && text.Length == 0) {
                    return 2;
                }

                int n = text.Length;
                int i = 0;
                int j = 0;

                while (i < n) {
                    if (pattern[j] == text[i]) {
                        i++;
                        j++;
                    }
                    if (j == m) {
                        return i - j;
                    } else if (i < n && pattern[j] != text[i]) {
                        if (j != 0) {
                            j = b[j - 1];
                        } else {
                            i++;
                        }
                    }
                }
            }

            // If no match is found, return -1
            return -1;
        }
    }
}
