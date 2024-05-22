from PIL import Image
import os, json


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

def image_to_ascii(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    # Get the center 30x30 box
    start_x = width // 2 - 3
    start_y = height // 2 - 2
    crop_box = (start_x, start_y, start_x + 6, start_y + 5)
    cropped_img = img.crop(crop_box)

    # Convert the 30x30 image to ASCII
    binary_data = ''
    for y in range(5):
        for x in range(6):
            pixel_value = cropped_img.getpixel((x, y))
            # Create a binary string based on the pixel value
            binary_data += '0' if pixel_value < 128 else '1'
    print(binary_data)

    # Convert the binary data to ASCII
    ascii_data = ''
    for i in range(0, len(binary_data), 8):
        byte = binary_data[i:i+8]
        if len(byte) == 8:
            ascii_char = chr(int(byte, 2))
            ascii_data += ascii_char

    return ascii_data

# # Contoh penggunaan
source_img = 'data/598__M_Left_ring_finger.bmp'

def test_all_images_in_folder(folder_path, method):
    # Get all files in the folder
    files = os.listdir(folder_path)

    # Filter only .bmp files
    bmp_files = [file for file in files if file.endswith('.BMP')]

    pattern = image_to_ascii(source_img)
    # Apply the function to all .bmp files
    for bmp_file in bmp_files:
        image_path = os.path.join(folder_path, bmp_file)
        ascii_data = image_to_ascii(image_path)

        # Use KMP to match the string
        if method == 'kmp':
            kmp = KMP(pattern, ascii_data)
            result = kmp.search()
        else:
            bm = BM(pattern)
            result = bm.search(ascii_data)
        if result != -1:
            print(f'{bmp_file} is similar to {source_img} at index {result}')
            
import time
start = time.time()
char = image_to_ascii("data/599__M_Right_thumb_finger.bmp")
end = time.time()
print(char)
print(f'BM: {end - start} seconds')
