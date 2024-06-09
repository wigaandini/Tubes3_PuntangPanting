<h1 align="center"> Tugas Besar 3 IF2211 Strategi Algoritma </h1>
<h1 align="center">  Pattern Matching in Biometric Fingerprint Detection System using WPF C# and MySQL </h1>

![foto](img/foto.jpg)

<br/>

## Introduction

The Pattern Matching in Biometric Fingerprint Detection System is a sophisticated application designed to provide efficient biometric authentication using fingerprint images. Developed using WPF C# and MySQL, this system leverages advanced pattern matching algorithms to accurately identify individuals based on their unique fingerprint patterns. The pattern matching algorithm is using Boyer Moore and Knuth Morris Pratt for searching the exactly match 100% fingerprint. If there is no fingerprint that exactly match, the percentage is counted using Levensthein algorithm.

## Table of Contents

- [Depedencies](#depedencies)
- [Concept](#concept)
- [Features](#features)
- [Screenshots](#screenshots)
- [Development](#development)
- [Project Status](#project-status)
- [Room for Improvement](#room-for-improvement)
- [Acknowledgements](#acknowledgements)
- [Mainteners](#mainteners)


## Dependencies

- **Bogus (Version 35.5.1)**
  Bogus is used for generating fake data.

- **MySql.Data (Version 8.4.0)**
  MySql.Data is the official MySQL database driver for .NET, necessary for interacting with MySQL databases.

- **System.Drawing.Common (Version 8.0.6)**
  System.Drawing.Common provides access to GDI+ for image processing functionalities in application.


## Concept

### Knuth-Morris-Pratt (KMP) Algorithm

The KMP algorithm efficiently finds patterns in text by precomputing a border array that helps skip unnecessary comparisons during string matching.

- **Border Array:** Generated using the `Border` method, the border array represents the longest prefix that is also a suffix for each prefix of the pattern. It aids in optimizing the pattern matching process by allowing the algorithm to know how much to shift the pattern index when a mismatch occurs.
- **String Matching:** The `Match` method utilizes the border array to efficiently match patterns in text. It iterates through the text and pattern, adjusting the pattern index based on the border array values, which significantly reduces the number of comparisons required.

### Boyer-Moore (BM) Algorithm

Boyer-Moore is a string matching algorithm that skips comparisons based on a last occurrence table for characters in the pattern.

- **Last Occurrence Table:** Created by the `LastOccurrence` method, the last occurrence table stores the index of the last occurrence of each character in the pattern. It guides efficient pattern matching by determining the amount of shift needed when a mismatch occurs.
- **String Matching:** The `Match` method implements Boyer-Moore for efficient pattern matching. It starts matching from the end of the pattern and adjusts the index based on the last occurrence table, which helps avoid unnecessary comparisons and improves overall performance.


### Regex
- **Normalization:**
   - Both sentences are normalized by converting them to lowercase, removing non-alphabetic and non-numeric characters, and applying an "alay" normalization using the `AlayNormalization` method.
- **Word Comparison:**
   - The sentences are split into arrays of words, and if their lengths are unequal, the method returns false immediately.
   - For each corresponding word pair in the arrays, a nested function called `RegexExtractVokalAndCompare` is used to extract vowels and compare the non-vowel portions.
   - This function employs regex to extract vowels from both words and checks if the non-vowel parts match after removing vowels.
   - If any word pair fails this comparison, the method returns false.
   - If all word pairs pass the comparison, indicating matching vowel patterns and non-vowel sections, the method returns true, signifying that the sentences are equivalent based on the specified criteria.


## Features

### Multithreading

- **Parallel Processing:** The application utilizes multithreading to perform parallel processing tasks, enabling faster execution and improved performance.
- **Concurrency:** Multiple threads are utilized to handle concurrent tasks efficiently, allowing for better resource utilization and responsiveness.

### Encrypt/Decrypt Using AES (Advanced Encryption Standard)

- **Secure Encryption:** The application implements AES encryption for secure data encryption, ensuring confidentiality and integrity of sensitive information.
- **AES Key Management:** Users can generate, manage, and securely store AES keys for encrypting and decrypting data.
- **Efficient Decryption:** AES decryption functionality allows for the secure retrieval of encrypted data, providing confidentiality while ensuring data integrity.

### Seeder

- **Data Population:** The seeder feature facilitates the population of databases or data stores with sample or test data.
- **Customizable:** Users can configure the seeder to generate specific types of data


## Development

To run the application locally:

### Prerequisites

Before starting the development process, ensure that you have the following software installed on your machine:

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) (programming language)
- [MySQL](https://dev.mysql.com/doc/) or [MariaDB](https://mariadb.org/) (database management system)
- [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/) (Windows Presentation Foundation)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/) (integrated development environments for C#)

### Steps to Run the Application

1. **Clone the Repository**

    ```bash
    git clone https://github.com/wigaandini/Tubes3_PuntangPanting.git
    cd Tubes3_PuntangPanting
    ```

2. **Navigate to the Source (src) Directory**

    ```bash
    cd src
    ```

#### To Run the Seeder

3. **Navigate to Seeder Directory**

    ```bash
    cd "Tubes3 PuntangPanting/DatabaseSeeder"
    ```

4. **Install Dependencies**

    ```bash
    dotnet restore
    ```

5. **Build**

    ```bash
    dotnet build
    ```

6. **Run**

    ```bash
    dotnet run
    ```

#### To Run the Main Application

3. **Navigate to Main Application Directory**

    ```bash
    cd "Tubes3 PuntangPanting/Tubes3 PuntangPanting"
    ```

4. **Install Dependencies**

    ```bash
    dotnet restore
    ```

5. **Build**

    ```bash
    dotnet build
    ```

6. **Run**

    ```bash
    dotnet run
    ```

## Project Status

Project is complete

## Maintainers

| NIM      | Nama                      |
| -------- | ------------------------- |
| 13522053 | Erdianti Wiga Putri Andini|
| 13522057 | Moh Fairuz Alauddin Yahya |
| 13522097 | Ellijah Darrellshane S	   |
