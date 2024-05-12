using System;

namespace Algo {
    public class BMAlgo {
        private static int[] LastOccurrence(string text) {
            int[] last = new int[128];

            for (int i = 0; i < 128; i++) {
                last[i] = -1;
            }

            for (int i = 0; i < text.Length; i++) {
                last[text[i]] = i;
            }

            return last;
        }

        public static int BMMatch(string pattern, string text) {
            int[] last = LastOccurrence(text);
            int n = text.Length;
            int m = pattern.Length;
            int i = m - 1;

            if (i > n - 1) {
                return -1;
            }
            else {
                int j = m - 1;
                while (true) {
                    if (pattern[j] == text[i]) {
                        if (j == 0)
                        {
                            return i;
                        }
                        else
                        {
                            i--;
                            j--;
                        }
                    }
                    else {
                        int lo = last[text[i]];
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
