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

# def specific_image_ascii(image_path):
#     # Buka gambar
#     img = Image.open(image_path)
#     width, height = img.size

#     # Grayscaling image
#     img = img.convert('L')

#     # Get the center 30x30 box
#     start_x = width // 2 - 4
#     start_y = height // 2 - 2
#     crop_box = (start_x, start_y, start_x + 8, start_y + 4)
#     cropped_img = img.crop(crop_box)

#     binary_data = ''
#     for y in range(4):
#         for x in range(8):
#             pixel_value = cropped_img.getpixel((x, y))
#             # Create a binary string based on the pixel value
#             binary_data += '0' if pixel_value < 128 else '1'

#     ascii_data = ''
#     for i in range(0, len(binary_data), 8):
#         byte = binary_data[i:i+8]
#         if len(byte) == 8:
#             ascii_char = chr(int(byte, 2))
#             ascii_data += ascii_char

#     return ascii_data


# from PIL import Image

# def specific_image_ascii(image_path, sub_pixels, heigh_pixels):
#     # Buka gambar
#     img = Image.open(image_path)
#     width, height = img.size

#     # Grayscaling image
#     img = img.convert('L')

#     # Ensure the crop box is centered and has the dimensions of sub_pixels x heigh_pixels
#     start_x = (width - sub_pixels) // 2
#     start_y = (height - heigh_pixels) // 2
#     crop_box = (start_x, start_y, start_x + sub_pixels, start_y + heigh_pixels)
#     cropped_img = img.crop(crop_box)

#     binary_data = ''
#     for y in range(heigh_pixels):
#         for x in range(sub_pixels):
#             pixel_value = cropped_img.getpixel((x, y))
#             # Create a binary string based on the pixel value
#             binary_data += '0' if pixel_value < 128 else '1'

#     ascii_data = ''
#     for i in range(0, len(binary_data), 8):
#         byte = binary_data[i:i+8]
#         if len(byte) == 8:
#             ascii_char = chr(int(byte, 2))
#             ascii_data += ascii_char

#     return ascii_data

# specific_image_ascii('path_to_image.jpg', 8, 4)

from PIL import Image

def specific_image_ascii(image_path, target_width=8, target_height=4):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Resize image jika ukuran tidak habis dibagi target width dan height
    if width % target_width != 0:
        new_width = (width // target_width + 1) * target_width
    else:
        new_width = width
    if  height % target_height != 0:
        new_height = (height // target_height + 1) * target_height
    else:
        new_height = height
        
    img = img.resize((new_width, new_height), Image.LANCZOS)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    # Get the center target_width x target_height box
    start_x = (width - target_width) // 2
    start_y = (height - target_height) // 2
    crop_box = (start_x, start_y, start_x + target_width, start_y + target_height)
    cropped_img = img.crop(crop_box)

    binary_data = ''
    for y in range(target_height):
        for x in range(target_width):
            pixel_value = cropped_img.getpixel((x, y))
            # Create a binary string based on the pixel value
            binary_data += '0' if pixel_value < 128 else '1'

    ascii_data = ''
    for i in range(0, len(binary_data), 8):
        byte = binary_data[i:i+8]
        if len(byte) == 8:
            ascii_char = chr(int(byte, 2))
            ascii_data += ascii_char

    return ascii_data

def image_ascii(image_path, target_width=8, target_height=4):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Resize image jika ukuran tidak habis dibagi target width dan height
    if width % target_width != 0:
        new_width = (width // target_width + 1) * target_width
    else:
        new_width = width
    if height % target_height != 0:
        new_height = (height // target_height + 1) * target_height
    else:
        new_height = height

    img = img.resize((new_width, new_height), Image.LANCZOS)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    ascii_data = ''

    # Get the center box
    start_x = (width - target_width) // 2
    start_y = (height - target_height) // 2
    step = 1  # Langkah positif untuk ke kanan, langkah negatif untuk ke kiri

    for y in range(start_y, start_y + height, target_height):
        for x in range(start_x, start_x + width, target_width * step):
            crop_box = (x, y, x + target_width, y + target_height)
            cropped_img = img.crop(crop_box)

            binary_data = ''
            for y in range(target_height):
                for x in range(target_width):
                    pixel_value = cropped_img.getpixel((x, y))
                    # Create a binary string based on the pixel value
                    binary_data += '0' if pixel_value < 128 else '1'

            for i in range(0, len(binary_data), 8):
                byte = binary_data[i:i+8]
                if len(byte) == 8:
                    ascii_char = chr(int(byte, 2))
                    ascii_data += ascii_char

        # Ubah arah langkah setelah selesai iterasi ke kanan
        step *= -1

    return ascii_data

def image_asciii(image_path, target_width=8, target_height=4):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    ascii_data = ''

    for start_y in range(height - target_height + 1):
        for start_x in range(width - target_width + 1):
            binary_data = ''
            for y in range(target_height):
                for x in range(target_width):
                    pixel_value = img.getpixel((start_x + x, start_y + y))
                    # Create a binary string based on the pixel value
                    binary_data += '0' if pixel_value < 128 else '1'

            for i in range(0, len(binary_data), 8):
                byte = binary_data[i:i+8]
                if len(byte) == 8:
                    ascii_char = chr(int(byte, 2))
                    ascii_data += ascii_char
    
    return ascii_data

def find_dominant_dark_area(img):
    width, height = img.size
    window_size = 8  # Adjust this as needed for the size of the dark area
    min_intensity_sum = float('inf')
    dominant_coords = (0, 0)

    for y in range(height - window_size): 
        for x in range(width - window_size):  
            intensity_sum = sum(
                sum(img.getpixel((x + dx, y + dy))[0:3])
                for dy in range(window_size)
                for dx in range(window_size)
            )
            if intensity_sum < min_intensity_sum:
                min_intensity_sum = intensity_sum
                dominant_coords = (x, y)

    return dominant_coords

def highest_dark_ascii(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Cari area dengan konsentrasi piksel tertinggi
    max_intensity_coords = find_dominant_dark_area(img)

    # Gunakan koordinat tersebut untuk crop gambar sebesar 8x4
    crop_box = (max_intensity_coords[0], max_intensity_coords[1],
                max_intensity_coords[0] + 8, max_intensity_coords[1] + 4)
    cropped_img = img.crop(crop_box)

    # Convert cropped image to grayscale
    cropped_img = cropped_img.convert('L')

    # Convert the 8x4 image to ASCII
    binary_data = ''
    for y in range(4):  # Ubah dari 30 ke 4 untuk tinggi
        for x in range(8):  # Ubah dari 30 ke 8 untuk lebar
            pixel_value = cropped_img.getpixel((x, y))
            # Create a binary string based on the pixel value
            binary_data += '0' if pixel_value < 128 else '1'

    # Convert the binary data to ASCII
    ascii_data = ''
    for i in range(0, len(binary_data), 8):
        byte = binary_data[i:i+8]
        if len(byte) == 8:
            ascii_char = chr(int(byte, 2))
            ascii_data += ascii_char

    return ascii_data


# # Contoh penggunaan
# Contoh pemanggilan fungsi
source_img = 'src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/1__M_Left_little_finger.BMP'
source_img2 = 'src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/1__M_Left_little_finger.BMP'
ascii_art = image_asciii(source_img)
ascii_art_2 = highest_dark_ascii(source_img2)
def bm_search_timing():
    bm = BM(source_img)
    result = bm.search(source_img2)
    if result != -1:
        print("bener kok")
import timeit
setup_code = """
from __main__ import BM, source_img, source_img2
"""
execution_time = timeit.timeit(stmt=bm_search_timing, setup=setup_code, number=1)

# Convert to milliseconds
execution_time_ms = execution_time * 1000

# Print the result
print(f'BM Search Execution Time: {execution_time_ms} milliseconds')
def test_all_images_in_folder(folder_path, method):
    # Get all files in the folder
    files = os.listdir(folder_path)

    # Filter only .bmp files
    bmp_files = [file for file in files if file.endswith('.BMP')]

    pattern = specific_image_ascii(source_img)
    print(pattern)
    # Apply the function to all .bmp files
    for bmp_file in bmp_files:
        image_path = os.path.join(folder_path, bmp_file)
        ascii_data = image_asciii(image_path)
        

        # Use KMP to match the string
        if method == 'kmp':
            kmp = KMP(pattern, ascii_data)
            result = kmp.search()
        else:
            bm = BM(pattern)
            result = bm.search(ascii_data)
        if result != -1:
            print(f'{bmp_file} is similar to {source_img} at index {result}')
            print(ascii_data)
            
# import time
# start = time.time()
# char = test_all_images_in_folder("src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/", "kmpS")
# end = time.time()
# print(char)
# print(f'BM: {end - start} seconds')
