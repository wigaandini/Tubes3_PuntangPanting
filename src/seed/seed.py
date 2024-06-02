import mysql.connector
from faker import Faker
import random
import os
import re
from ascii_converter import image_ascii


class DatabaseManager:
    def __init__(self, host, user, password, database):
        self.host = host
        self.user = user
        self.password = password
        self.database = database
        self.connection = None
        self.cursor = None

    def connect(self):
        self.connection = mysql.connector.connect(
            host=self.host,
            user=self.user,
            password=self.password,
            database=self.database,
            connection_timeout=10000000,
        )
        self.cursor = self.connection.cursor()

    def disconnect(self):
        if self.cursor:
            self.cursor.close()
        if self.connection:
            self.connection.close()

    def create_tables(self):
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
          `berkas_citra` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
          `nama` varchar(100) DEFAULT NULL,
          `path` text NOT NULL,
          PRIMARY KEY (`path`(255))
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        """

        self.cursor.execute(create_biodata_table)
        self.cursor.execute(create_sidik_jari_table)
        self.connection.commit()

    def generate_unique_name(self, fake):
        fake_name = fake.first_name() + " " + fake.last_name()
        cleaned_name = self.remove_non_alphabetic(fake_name)
        real_name = self.remove_titles(cleaned_name)
        return real_name

    def remove_non_alphabetic(self, name):
        cleaned_name = "".join(c for c in name if c.isalpha() or c.isspace())
        return cleaned_name

    def remove_titles(self, name):
        titles = [
            "SE",
            "SEI",
            "SPsi",
            "MM",
            "SPt",
            "MPd",
            "Dr",
            "Drs",
            "Ir",
            "SH",
            "MH",
            "MSi",
            "MHum",
            "MA",
            "MSc",
            "PhD",
            "Prof",
            "Prof Dr",
            "Dr Hc",
            "BSc",
            "BA",
            "BEng",
            "MBA",
            "LLB",
            "LLM",
            "MPhil",
            "DPhil",
            "EdD",
            "DDS",
            "DMD",
            "DO",
            "DVM",
            "MD",
            "MFA",
            "JD",
            "PsyD",
            "ThD",
            "DMin",
            "BBA",
            "AB",
            "BS",
            "BM",
            "BFA",
            "MLIS",
            "MSW",
            "MPH",
            "MEd",
            "MEng",
            "MArch",
            "MDes",
            "MSN",
            "DSc",
            "DHEd",
            "DMus",
            "DPT",
            "OTD",
            "PharmD",
            "RN",
            "NP",
            "CFA",
            "CPA",
            "Esq",
            "PGDip",
            "DipHE",
            "CertHE",
            "PGCE",
            "BEd",
            "MSt",
            "MMus",
            "MAEd",
            "MChem",
            "MBiol",
            "MSocSci",
            "BAA",
            "BAppSc",
            "MComp",
            "MAcc",
            "AMd",
            "SAg",
            "SPd",
            "SE",
            "SKom",
            "SHum",
            "SIKom",
            "SPt",
            "SFarm",
            "SKed",
            "SKes",
            "SSi",
            "SKH",
            "ST",
            "STP",
            "SPsi",
            "SPt",
            "SSos",
            "SH",
            "SHut",
            "SPi",
            "SGz",
            "SStat",
            "STrT",
            "STrKes",
            "MAg",
            "MPd",
            "MSi",
            "MKom",
            "MHum",
            "MIKom",
            "MFarm",
            "MKed",
            "MKes",
            "MSi",
            "MKH",
            "MT",
            "MTP",
            "MPsi",
            "MSos",
            "MH",
            "MHut",
            "MPi",
            "MGz",
            "MStat",
            "MTrT",
            "MTrKes",
            "DS",
            "DAg",
            "DPd",
            "DSi",
            "DKom",
            "DHum",
            "DIKom",
            "DFarm",
            "DKed",
            "DKes",
            "DT",
            "DTP",
            "DPsi",
            "DSos",
            "DH",
            "DHut",
            "DPi",
            "DGz",
            "DStat",
            "DTrT",
            "DTrKes",
            "MTi",
            "Mak, msip",
            "Mak, msi",
        ]
        name_lower = name.lower()
        titles_lower = [title.lower() for title in titles]
        pattern = r"\b(" + "|".join(titles_lower) + r")\b"
        cleaned_name = re.sub(pattern, "", name_lower).strip()
        cleaned_name = cleaned_name.title()
        cleaned_name = re.sub(r"\s+", " ", cleaned_name)
        return cleaned_name

    def generate_random_corrupt_name(self, name):
        corrupt_name = self.random_remove_vocal(name)
        corrupt_name = self.random_case(corrupt_name)
        corrupt_name = self.random_alay(corrupt_name)
        return corrupt_name

    def random_remove_vocal(self, char):
        result = ""
        for c in char:
            char_lower = c.lower()
            if char_lower in ["a", "i", "u", "e", "o"]:
                remove = random.choice([True, False])
                if not remove:
                    result += c
            else:
                result += c
        return result

    def random_case(self, char):
        result = ""
        for c in char:
            change = random.choice([True, False])
            if change:
                result += c.upper()
            else:
                result += c.lower()
        return result

    def random_alay(self, char):
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

    def generate_fake_data(self, fake, image_folder_path):
        files = os.listdir(image_folder_path)
        bmp_files = [file for file in files if file.endswith(".BMP")]
        total_data = len(bmp_files)
        biodata_records = []
        sidik_jari_records = []
        unique_paths = set()
        i = 0
        while i < len(bmp_files):
            real_name = self.generate_unique_name(fake)
            NIK = str(fake.random_number(digits=16, fix_len=True))
            nama = self.generate_random_corrupt_name(real_name)
            tempat_lahir = fake.city()
            tanggal_lahir = fake.date_of_birth()
            jenis_kelamin = fake.random_element(elements=("Laki-Laki", "Perempuan"))
            golongan_darah = fake.random_element(elements=("A", "B", "AB", "O"))
            alamat = fake.address()
            agama = fake.random_element(
                elements=("Islam", "Kristen", "Katolik", "Hindu", "Buddha")
            )
            status_perkawinan = fake.random_element(
                elements=("Belum Menikah", "Menikah", "Cerai")
            )
            pekerjaan = fake.job()
            kewarganegaraan = fake.random_element(elements=("WNI", "WNA"))

            biodata_data = (
                NIK,
                nama,
                tempat_lahir,
                tanggal_lahir,
                jenis_kelamin,
                golongan_darah,
                alamat,
                agama,
                status_perkawinan,
                pekerjaan,
                kewarganegaraan,
            )
            biodata_records.append(biodata_data)

            remaining_records = total_data - i
            rand = random.randint(1, 5)
            random_count = min(rand, remaining_records)
            for j in range(random_count):
                bmp_file = bmp_files[i + j]
                path = "data/" + bmp_file
                berkas_citra = image_ascii(os.path.join(image_folder_path, bmp_file))

                if path not in unique_paths:
                    sidik_jari_data = (berkas_citra, real_name, path)
                    sidik_jari_records.append(sidik_jari_data)
                    unique_paths.add(path)
                else:
                    print(f"Duplicate path detected: {path}")

            i += random_count
            print(f"Generated {i}/{total_data} records")

        return biodata_records, sidik_jari_records

    def insert_fake_data(self, biodata_records, sidik_jari_records):
        insert_biodata = """
        INSERT INTO biodata (
            NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah,
            alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan
        ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        """

        insert_sidik_jari = """
        INSERT INTO sidik_jari (berkas_citra,nama, path) VALUES (%s,%s, %s)
        """

        self.cursor.executemany(insert_biodata, biodata_records)
        self.cursor.executemany(insert_sidik_jari, sidik_jari_records)
        self.connection.commit()


if __name__ == "__main__":
    # Initialize Faker and DatabaseManager
    fake = Faker("id_ID")
    db_manager = DatabaseManager(
        host="Fairuz",
        user="root",
        password="bismillah.33",
        database="stima",
    )

    # Connect to the database
    db_manager.connect()

    # Create tables if they don't exist
    db_manager.create_tables()

    # Generate fake data
    biodata_records, sidik_jari_records = db_manager.generate_fake_data(
        fake, "../Tubes3 PuntangPanting/Tubes3 PuntangPanting/data"
    )

    # Insert fake data into the database
    db_manager.insert_fake_data(biodata_records, sidik_jari_records)

    # Disconnect from the database
    db_manager.disconnect()
