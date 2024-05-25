from PIL import Image, ImageDraw
import os

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

def add_highlight_and_border(image_path):
    # Open the image
    img = Image.open(image_path)
    width, height = img.size

    # Find the dominant dark area
    dominant_coords = find_dominant_dark_area(img)

    # Create a copy of the image and a drawing object
    highlighted_img = img.copy()
    draw = ImageDraw.Draw(highlighted_img)

    # Define the coordinates for the highlight rectangle
    highlight_coords = (
        dominant_coords[0],
        dominant_coords[1],
        dominant_coords[0] + 8,
        dominant_coords[1] + 8  # Adjusted to match the window size
    )

    # Draw the highlight rectangle
    draw.rectangle(highlight_coords, outline='yellow', width=2)

    # Add a red border around the highlighted area
    border_coords = (
        dominant_coords[0] - 2,
        dominant_coords[1] - 2,
        dominant_coords[0] + 10,
        dominant_coords[1] + 10  # Adjusted to match the window size
    )
    draw.rectangle(border_coords, outline='red', width=2)

    return highlighted_img


def image_to_ascii(image_path):
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


source_img = 'src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/10__M_Right_ring_finger.BMP'

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
char = test_all_images_in_folder("src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data", "bm")
end = time.time()
print(char)
print(f'BM: {end - start} seconds')
