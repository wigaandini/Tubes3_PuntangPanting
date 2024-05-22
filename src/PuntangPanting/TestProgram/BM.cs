using System;
using System.Collections.Generic;

namespace TestProgram {
    public class BMAlgo {
        private static Dictionary<char, int> LastOccurrence(string pattern) {
            Dictionary<char, int> last = new Dictionary<char, int>();

            for (int i = 0; i < pattern.Length; i++) {
                last[pattern[i]] = i;
            }

            return last;
        }

        public static int Match(string pattern, List<string> texts) {
            if (texts.Count == 0) {
                return -1;
            }

            Dictionary<char, int> last = LastOccurrence(pattern);

            foreach (string text in texts) {
                int n = text.Length;
                int m = pattern.Length;
                int i = m - 1;
                
                if (pattern.Length == 0 && text.Length == 0) {
                    return 2;
                }
                
                while (i < n) {
                    int j = m - 1;

                    while (j >= 0 && pattern[j] == text[i]) {
                        i--;
                        j--;
                    }

                    if (j < 0) {
                        return i + 1; // Match found
                    } else {
                        int lo = last.ContainsKey(text[i]) ? last[text[i]] : -1;
                        i += m - Math.Min(j, 1 + lo);
                    }
                }
            }
            
            // If no match is found, return -1
            return -1;
        }

    }
}