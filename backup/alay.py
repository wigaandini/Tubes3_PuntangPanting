import re


def num_to_char(teks):
    return re.sub(r'([^\d]*)([012345789])', lambda match: match.group(1) + {
        "1": "i",
        "2": "z",
        "3": "e",
        "4": "a",
        "5": "s",
        "7": "t",
        "8": "b",
        "9": "g",
        "0": "o",
    }[match.group(2)], teks)

def alay_normalization(teks):
    teks = teks.lower()
    teks = num_to_char(teks)
    return teks

def remove_vokal(teks):
    return re.sub(r'[aiueo]', '', teks)

def compare_word(sentence, source_sentence):
    def regex_extract_vokal_and_compare(word, source):
        vokal_word = re.findall(r'[aiueo]', word)
        vokal_source = re.findall(r'[aiueo]', source)
        no_vokal_word = re.sub(r'[aiueo]', '', word)
        no_vokal_source = re.sub(r'[aiueo]', '', source)

        if no_vokal_word != no_vokal_source:
            return False

        if len(vokal_word) > len(vokal_source):
            longer, shorter = vokal_word, vokal_source
        else:
            longer, shorter = vokal_source, vokal_word

        pattern = f"[{''.join(longer)}]"
        return all(re.search(pattern, char) for char in shorter)
    
    array_word = sentence.split()
    array_source = source_sentence.split()
    
    if len(array_word) != len(array_source):
        return False

    return all(regex_extract_vokal_and_compare(word, source) for word, source in zip(array_word, array_source))


teks_alay = "dn 4dl omo"
teks_asli = "dania adalia oemaoe"
variasi_teks_normal = alay_normalization(teks_alay)

print(variasi_teks_normal)

# Bandingkan kata dalam teks normalisasi dengan teks asli
print("\nHasil Compare:")
print(compare_word(variasi_teks_normal, teks_asli))
