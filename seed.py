import mysql.connector
from faker import Faker
import random
from PIL import Image
import os, re

# Inisialisasi objek Faker
fake = Faker("id_ID")

titles = [
    "SE", "SEI", "SPsi", "MM", "SPt", "MPd", 
    "Dr", "Drs", "Ir", "SH", "MH", "MSi", 
    "MHum", "MA", "MSc", "PhD", "Prof", "Prof Dr", 
    "Dr Hc", "BSc", "BA", "BEng", "MBA", "LLB", 
    "LLM", "MPhil", "DPhil", "EdD", "DDS", "DMD", 
    "DO", "DVM", "MD", "MFA", "JD", "PsyD", "ThD", 
    "DMin", "BBA", "AB", "BS", "BM", "BFA", "MLIS", 
    "MSW", "MPH", "MEd", "MEng", "MArch", "MDes", 
    "MSN", "DSc", "DHEd", "DMus", "DPT", "OTD", 
    "PharmD", "RN", "NP", "CFA", "CPA", "Esq", 
    "PGDip", "DipHE", "CertHE", "PGCE", "BEd", 
    "MSt", "MMus", "MAEd", "MChem", "MBiol", 
    "MSocSci", "BAA", "BAppSc", "MComp", "MAcc", 
    "AMd", "SAg", "SPd", "SE", "SKom", "SHum", 
    "SIKom", "SPt", "SFarm", "SKed", "SKes", 
    "SSi", "SKH", "ST", "STP", "SPsi", "SPt",
    "SSos", "SH", "SHut", "SPi", "SGz", "SStat", 
    "STrT", "STrKes", "MAg", "MPd", "MSi", 
    "MKom", "MHum", "MIKom", "MFarm", "MKed", 
    "MKes", "MSi", "MKH", "MT", "MTP", "MPsi", 
    "MSos", "MH", "MHut", "MPi", "MGz", "MStat", 
    "MTrT", "MTrKes", "DS", "DAg", "DPd", "DSi", 
    "DKom", "DHum", "DIKom", "DFarm", "DKed", 
    "DKes", "DT", "DTP", "DPsi", "DSos", "DH", 
    "DHut", "DPi", "DGz", "DStat", "DTrT", 
    "DTrKes", "MTi", "Mak, msip", "Mak, msi",
]
def random_case(char):
    result = ""
    for c in char:
        change = random.choice([True, False])
        if change:
            result += c.upper()
        else:
            result += c.lower()
    return result

def remove_non_alphabetic(name):
    cleaned_name = ''.join(c for c in name if c.isalpha() or c.isspace())
    return cleaned_name

def remove_titles(name):
    # Convert the name to lowercase for case-insensitive matching
    name_lower = name.lower()
    titles_lower = [title.lower() for title in titles]
    pattern = r"\b(" + "|".join(titles_lower) + r")\b"
    cleaned_name = re.sub(pattern, "", name_lower).strip()
    # Capitalize the first letter of each word
    cleaned_name = cleaned_name.title()
    # Remove any extra whitespace
    cleaned_name = re.sub(r'\s+', ' ', cleaned_name)
    return cleaned_name

def random_remove_vocal(char):
    result = ""
    for c in char:
        char_lower = c.lower()
        if char_lower in ['a', 'i', 'u', 'e', 'o']:
            remove = random.choice([True, False])
            if not remove:
                result += c
        else:
            result += c
    return result

def random_alay(char):
    mapping = {
        "i": "1",
        "z": "2",
        "e": "3",
        "a": "4",
        "s": "5",
        "t": "7",
        "b": "8",
        "g": "9",
        "o": "0",
    }
    result = ""
    for c in char:
        if c in mapping:
            change = random.choice([True, False])
            if change:
                result += mapping[c]
            else:
                result += c
        else:
            result += c
    return result

used_names = set()

def generate_unique_name(fake):
        fake_name = fake.name()
        cleaned_name = remove_non_alphabetic(fake_name)
        real_name = remove_titles(cleaned_name)
        return real_name
        

def generate_random_corrupt_name(name):
    corrupt_name = random_remove_vocal(name)
    corrupt_name = random_case(corrupt_name)
    corrupt_name = random_alay(corrupt_name)
    return corrupt_name

def image_to_ascii(image_path):
    # Buka gambar
    img = Image.open(image_path)
    width, height = img.size

    # Grayscaling image
    img = img.convert('L')

    # Get the center 30x30 box
    start_x = width // 2 - 15
    start_y = height // 2 - 15
    crop_box = (start_x, start_y, start_x + 30, start_y + 30)
    cropped_img = img.crop(crop_box)

    # Convert the 30x30 image to ASCII
    binary_data = ''
    for y in range(30):
        for x in range(30):
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

# Connect to your MySQL database
db_connection = mysql.connector.connect(
    host="Fairuz",
    user="root",
    password="bismillah.33",
    database="stima"
)

# Create a cursor object to execute SQL queries
cursor = db_connection.cursor()

# Define SQL statements to create tables
create_biodata_table = """
CREATE TABLE IF NOT EXISTS `biodata` (
  `NIK` varchar(16) NOT NULL,
  `nama` varchar(100) DEFAULT NULL,
  `tempat_lahir` varchar(50) DEFAULT NULL,
  `tanggal_lahir` date DEFAULT NULL,
  `jenis_kelamin` enum('Laki-Laki','Perempuan') DEFAULT NULL,
  `golongan_darah` varchar(5) DEFAULT NULL,
  `alamat` varchar(255) DEFAULT NULL,
  `agama` varchar(50) DEFAULT NULL,
  `status_perkawinan` enum('Belum Menikah','Menikah','Cerai') DEFAULT NULL,
  `pekerjaan` varchar(100) DEFAULT NULL,
  `kewarganegaraan` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`NIK`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
"""

create_sidik_jari_table = """
CREATE TABLE IF NOT EXISTS `sidik_jari` (
  `berkas_citra` text DEFAULT NULL,
  `nama` varchar(100) DEFAULT NULL,
  `path` text NOT NULL,
  PRIMARY KEY (`path`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
"""

# Execute the SQL create table statements using cursor
cursor.execute(create_biodata_table)
cursor.execute(create_sidik_jari_table)
# Create a cursor object to execute SQL queries

# Path to the folder containing images
image_folder_path = "data"
files = os.listdir(image_folder_path)

# Filter only .bmp files
bmp_files = [file for file in files if file.endswith('.BMP')]

total_data = len(bmp_files)

# Collect data first
biodata_records = []
sidik_jari_records = []
unique_paths = set()
unique_names = set()
# Create one-to-many relationship
i = 0
while i < total_data:
    real_name = generate_unique_name(fake)
    NIK = str(fake.random_number(digits=16, fix_len=True))
    nama = generate_random_corrupt_name(real_name)
    tempat_lahir = fake.city()
    tanggal_lahir = fake.date_of_birth()
    jenis_kelamin = fake.random_element(elements=('Laki-Laki', 'Perempuan'))
    golongan_darah = fake.random_element(elements=('A', 'B', 'AB', 'O'))
    alamat = fake.address()
    agama = fake.random_element(elements=('Islam', 'Kristen', 'Katolik', 'Hindu', 'Buddha'))
    status_perkawinan = fake.random_element(elements=('Belum Menikah', 'Menikah', 'Cerai'))
    pekerjaan = fake.job()
    kewarganegaraan = fake.random_element(elements=('WNI', 'WNA'))

    biodata_data = (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
    biodata_records.append(biodata_data)

    # Create one-to-many relationship
    remaining_records = total_data - i
    rand = random.randint(1, 5)
    random_count = min(rand, remaining_records)  
    for j in range(random_count):
        bmp_file = bmp_files[i + j]
        berkas_citra = image_to_ascii(os.path.join(image_folder_path, bmp_file))
        path = os.path.join(image_folder_path, bmp_file)

        if path not in unique_paths:
            sidik_jari_data = (berkas_citra, real_name, path)
            sidik_jari_records.append(sidik_jari_data)
            unique_paths.add(path)
        else:
            print(f"Duplicate path detected: {path}")
    
    i += random_count

print(len(sidik_jari_records))

# Define SQL insert statements for biodata and sidik_jari
insert_biodata = """
INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
"""

insert_sidik_jari = """
INSERT INTO sidik_jari (berkas_citra, nama, path) VALUES (%s, %s, %s)
"""

# Execute the SQL insert statements using cursor
cursor.executemany(insert_biodata, biodata_records)
cursor.executemany(insert_sidik_jari, sidik_jari_records)

# Commit changes to the database
db_connection.commit()

# Close the cursor and database connection
cursor.close()
db_connection.close()