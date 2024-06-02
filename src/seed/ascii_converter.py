from PIL import Image

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