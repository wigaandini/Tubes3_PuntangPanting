from PIL import Image
import matplotlib.pyplot as plt
import matplotlib.patches as patches

from PIL import Image


# def is_unique_row(row, width):
#     color_pixel
#     for pixel in row:
#         if pixel != 0 and pixel != 255:
#             return False
#     return True

# def find_first_non_white_row(img, width, height):
#     for y in range(height):
#         row = [img.getpixel((x, y)) for x in range(width)]
#         if not is_unique_row(row, width):
#             return y
#     return 0

# def find_last_non_white_row(img, width, height):
#     for y in range(height-1, -1, -1):
#         row = [img.getpixel((x, y)) for x in range(width)]
#         if not is_unique_row(row, width):
#             return y
#     return height - 1

def find_dominant_dark_area(img, window_width=80, window_height=1):
    # Find first and last non-white rows
    width, height = img.size
    # first_non_white_row = find_first_non_white_row(img, width, height)
    # last_non_white_row = find_last_non_white_row(img, width, height)

    # # Crop the image to remove white rows from top and bottom
    # img = img.crop((0, first_non_white_row, width, last_non_white_row + 1))
    width, height = img.size  # Update dimensions after cropping

    min_intensity_sum = float('inf')
    dominant_coords = (0, 0)

    for y in range(1, height - window_height - 1):  # Avoid top and bottom borders
        for x in range(1, width - window_width - 1):  # Avoid left and right borders
            intensity_sum = sum(
                img.getpixel((x + dx, y + dy))
                for dy in range(window_height)
                for dx in range(window_width)
            )
            if intensity_sum < min_intensity_sum:
                min_intensity_sum = intensity_sum
                dominant_coords = (x, y)

    # # Create a rectangle patch to highlight the dominant dark area
    # rect = patches.Rectangle(dominant_coords, window_width, window_height, linewidth=1, edgecolor='r', facecolor='none')
    # fig, ax = plt.subplots()
    # ax.imshow(img, cmap='gray')
    # ax.add_patch(rect)
    # plt.show()
    return dominant_coords


def highest_dark_ascii(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size
    img = img.convert('L')
    # resize the image to make kelopatan 8 width
    if width % 8 != 0:
        img = img.resize((width - (width % 8), height))
    
    # Cari area dengan konsentrasi piksel tergelap
    max_intensity_coords = find_dominant_dark_area(img)

    # Gunakan koordinat tersebut untuk crop gambar sebesar 80x1
    crop_box = (max_intensity_coords[0], max_intensity_coords[1],
                max_intensity_coords[0] + 80, max_intensity_coords[1] + 1)
    cropped_img = img.crop(crop_box)

    # Convert the 80x1 image to ASCII
    binary_data = ''
    for x in range(80):  # Ubah dari 8 ke 80 untuk lebar
            pixel_value = cropped_img.getpixel((x, 0))
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

# import os
# from algo import KMP, BM
# source_img = 'src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/1__M_Left_little_finger.BMP'

# def test_all_images_in_folder(folder_path, method):
#     # Get all files in the folder
#     files = os.listdir(folder_path)

#     # Filter only .bmp files
#     bmp_files = [file for file in files if file.endswith('.BMP')]

#     pattern = mid_one(source_img)
#     # Apply the function to all .bmp files
#     for bmp_file in bmp_files:
#         image_path = os.path.join(folder_path, bmp_file)
#         ascii_data = image_ascii(image_path)
        
#         # Use KMP to match the string
#         if method == 'kmp':
#             kmp = KMP(pattern, ascii_data)
#             result = kmp.search()
#         else:
#             bm = BM(pattern)
#             result = bm.search(ascii_data)
#         if result != -1:
#             print(f'{bmp_file} is similar to {source_img} at index {result}')
            
# import time
# start = time.time()
# char = test_all_images_in_folder("src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/", "kmpS")
# end = time.time()
# print(char)
# print(f'BM: {end - start} seconds')