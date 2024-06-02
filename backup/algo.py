
class KMP:
    def __init__(self, pattern, text):
        self.pattern = pattern
        self.text = text
        self.construct_prefix_table()

    def construct_prefix_table(self):
        self.prefix_table = [0] * len(self.pattern)
        j = 0
        for i in range(1, len(self.pattern)):
            while j > 0 and self.pattern[i] != self.pattern[j]:
                j = self.prefix_table[j - 1]
            if self.pattern[i] == self.pattern[j]:
                j += 1
            self.prefix_table[i] = j

    def search(self):
        j = 0
        for i in range(len(self.text)):
            while j > 0 and self.text[i] != self.pattern[j]:
                j = self.prefix_table[j - 1]
            if self.text[i] == self.pattern[j]:
                j += 1
            if j == len(self.pattern):
                return i - (j - 1)
        return -1  # pattern not found

class BM:
    def __init__(self, pattern):
        self.pattern = pattern
        self.preprocess()

    def preprocess(self):
        self.bad_char_skip = [len(self.pattern)] * 256
        for i in range(len(self.pattern)):
            self.bad_char_skip[ord(self.pattern[i])] = len(self.pattern) - i - 1

    def search(self, text):
        i = 0
        while i <= len(text) - len(self.pattern):
            j = len(self.pattern) - 1
            while j >= 0 and text[i+j] == self.pattern[j]:
                j -= 1
            if j < 0:
                return i  # pattern found
            i += max(self.bad_char_skip[ord(text[i + len(self.pattern) - 1])], len(self.pattern) - j)
        return -1  # pattern not found
