from PIL import Image
import matplotlib.pyplot as plt
import matplotlib.patches as patches

from PIL import Image


def is_unique_row(row, width):
    for pixel in row:
        if pixel != 0 and pixel != 255:
            return False
    return True

def image_ascii(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Convert image to grayscale
    img = img.convert('L')
    if width % 8 != 0:
        img = img.resize((width - (width % 8), height))

    # # Find first and last non-white rows
    # first_non_white_row = find_first_non_white_row(img, width, height)
    # last_non_white_row = find_last_non_white_row(img, width, height)

    # # Crop the image to remove white rows from top and bottom
    # img = img.crop((0, first_non_white_row, width, last_non_white_row + 1))
    width, height = img.size  # Update dimensions after cropping

    ascii_data = ''
    binary_data = ''

    for y in range(height):
        for x in range(width):
            pixel_value = img.getpixel((x, y))
            # Create a binary string based on the pixel value
            binary_data += '0' if pixel_value < 128 else '1'
            
    for i in range(0, len(binary_data), 8):
        byte = binary_data[i:i+8]
        if len(byte) == 8:
            ascii_char = chr(int(byte, 2))
            ascii_data += ascii_char

    return ascii_data


def mid_one(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    # Dapatkan baris 1-pixel dari tengah gambar
    mid_height = height // 2

    # Tentukan titik awal dan akhir untuk crop 80 piksel di tengah sumbu X
    mid_width_start = max((width // 2) - 40, 0)
    mid_width_end = min(mid_width_start + 80, width)

    # Crop gambar untuk mendapatkan 80 piksel di tengah sumbu X
    crop_img = img.crop((mid_width_start, mid_height, mid_width_end, mid_height + 1))

    # Ubah gambar menjadi array nilai pixel
    pixels = list(crop_img.getdata())

    binary_data = ''

    # Proses setiap pixel
    for pixel in pixels:
        # Ubah pixel menjadi biner (0 jika di bawah 128, 1 jika 128 atau lebih)
        binary_value = '0' if pixel < 128 else '1'
        binary_data += binary_value

    # Konversi setiap 8 biner menjadi karakter ASCII
    ascii_data = ''
    for i in range(0, len(binary_data), 8):
        byte = binary_data[i:i+8]
        if len(byte) == 8:
            ascii_char = chr(int(byte, 2))
            ascii_data += ascii_char

    return ascii_data

import os
from algo import KMP, BM
from concurrent.futures import ThreadPoolExecutor
source_img = 'src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/1__M_Left_little_finger.BMP'

def process_image(image_path, pattern, method):
    ascii_data = image_ascii(image_path)
    if method == 'kmp':
        kmp = KMP(pattern, ascii_data)
        result = kmp.search()
    else:
        bm = BM(pattern)
        result = bm.search(ascii_data)
    if result != -1:
        print(f'{os.path.basename(image_path)} is similar to {source_img} at index {result}')
    return result

# Fungsi untuk memproses semua gambar di folder dengan multithreading
def test_all_images_in_folder(folder_path, method):
    files = os.listdir(folder_path)
    bmp_files = [os.path.join(folder_path, file) for file in files if file.endswith('.BMP')]
    pattern = mid_one(source_img)
    
    with ThreadPoolExecutor() as executor:
        futures = [executor.submit(process_image, bmp_file, pattern, method) for bmp_file in bmp_files]
        for future in futures:
            future.result()
            
import time
start = time.time()
char = test_all_images_in_folder("src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/", "kmpS")
end = time.time()
print(char)
print(f'BM: {end - start} seconds')