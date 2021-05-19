using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    public class DES
    {
        /// <summary>
        /// Zamiana char na ASCII
        /// </summary>
        /// <param name="c">Char</param>
        /// <returns>ASCII char code</returns>
        public int CharToASCII(char c)
        {
            return (int)c;
        }

        /// <summary>
        /// Zamiana liczby w systemie 10 na system 2 (wersja 8 bitowa)
        /// </summary>
        /// <param name="dec">Liczba zapisana w systemie 10</param>
        /// <returns>Liczba zapisana w systemie 2</returns>
        public string DecimalToBinary8Bits(int dec)
        {
            string bin = string.Empty;
            while (dec > 0)
            {
                int binaryNumber = dec % 2;
                dec = dec / 2;
                bin = binaryNumber.ToString() + bin;
            }

            while (bin.Length < 8)
            {
                bin = '0' + bin;
            }

            return bin;
        }

        /// <summary>
        /// Zamiana liczby w systemie 10 na system 2 (wersja 4 bitowa)
        /// </summary>
        /// <param name="dec">Liczba zapisana w systemie 10</param>
        /// <returns>Liczba zapisana w systemie 2</returns>
        public string DecimalToBinary4Bits(int dec)
        {
            string bin = string.Empty;
            while (dec > 0)
            {
                int binaryNumber = dec % 2;
                dec = dec / 2;
                bin = binaryNumber.ToString() + bin;
            }

            while (bin.Length < 4)
            {
                bin = '0' + bin;
            }

            return bin;
        }

        /// <summary>
        /// Zmiana stringa na ciąg bitów
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>Liczba binarna reprezentująca stringa</returns>
        public string StringToBin(string s)
        {
            var result = string.Empty;
            foreach (var character in s)
            {
                var ascii = CharToASCII(character);
                var bin = DecimalToBinary8Bits(ascii);
                result += bin;
            }
            return result;
        }

        /// <summary>
        /// Zmiana stringa bitów na napis
        /// </summary>
        /// <param name="s">String z bitami do zmiany na napis</param>
        /// <returns>Napis z ciągu bitów</returns>
        public string BinToString(string bin)
        {
            var result = string.Empty;
            for (int i = 0; i < bin.Length / 8; i += 1)
            {
                var character = bin.Substring(i * 8, 8);
                int intChar = Convert.ToInt32(character, 2);
                result += (char)intChar;
            }
            return result;
        }

        /// <summary>
        /// Zmiana stringa na bloki (Dodanie bajtów do niepełnego bloku)
        /// </summary>
        /// <param name="s">String do zamiany na bloki</param>
        /// <returns>String zamieniony na bloki 8-bitowe z ewentualnym dopisaniem bajtów by były równe 64-bitowe bloki</returns>
        public string AddBlocks(string s)
        {
            var bits = StringToBin(s);
            var mod = s.Length % 8;

            if (mod == 0)           // podzielne na 8
            {
                var bytesToAdd = 8;
                for (int i = 0; i < bytesToAdd; i++)
                {
                    bits += "00000000";
                }
            }
            else                    // nie podzielne na 8
            {
                var bytesToAdd = 8 - mod;
                for (int i = 0; i < bytesToAdd; i++)
                {
                    bits += "00000000";
                }
            }

            return bits;
        }

        /// <summary>
        /// Permutacja początkowa
        /// </summary>
        readonly int[] InitialPermutation =
        {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17,  9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        /// <summary>
        /// Zrobienie permutacji początkowej na ciągu bitów
        /// </summary>
        /// <param name="bits">Ciąg bitów</param>
        /// <returns>Ciąg bitów na którym została wykonana permutacja początkowa</returns>
        public string MakeInitialPermutation(string bits)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < InitialPermutation.Length; i++)
            {
                result.Append(bits[InitialPermutation[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Permutacja redukująca rozmiar klucza z 64 bitów do 56 bitów (permutowany wybór)
        /// </summary>
        readonly int[] PermutedChoice =
        {
            57, 49, 41, 33, 25, 17,  9,
             1, 58, 50, 42, 34, 26, 18,
            10,  2, 59, 51, 43, 35, 27,
            19, 11,  3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
             7, 62, 54, 46, 38, 30, 22,
            14,  6, 61, 53, 45, 37, 29,
            21, 13,  5, 28, 20, 12,  4
        };

        /// <summary>
        /// Zrobienie permutacji redukująca rozmiar klucza z 64 bitów do 56 bitów (permutowany wybór)
        /// </summary>
        /// <param name="bits">Ciąg bitów klucza</param>
        /// <returns>Ciąg bitów na którym został wykonany permutowany wybór</returns>
        public string MakePermutedChoice(string keyBits)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < PermutedChoice.Length; i++)
            {
                result.Append(keyBits[PermutedChoice[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Przesunięcia do wytworzenia podkluczy (subkey)
        /// </summary>
        readonly int[] tabelaPrzesuniec = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        /// <summary>
        /// Permuted Choice 2 do tworzenia podkluczy (subkey)
        /// </summary>
        readonly int[] PermutedChoice2 =
        {
            14, 17, 11, 24,  1,  5,
             3, 28, 15,  6, 21, 10,
            23, 19, 12,  4, 26,  8,
            16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        /// <summary>
        /// Zrobienie permutacji łączącej bloki C i D (Permuted Choice 2 – PC2)
        /// </summary>
        /// <param name="bits">Ciąg bitów klucza K</param>
        /// <returns>Ciąg bitów na którym został wykonany permutowany wybór</returns>
        public string MakePermutedChoice2(string keyBits)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < PermutedChoice2.Length; i++)
            {
                result.Append(keyBits[PermutedChoice2[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Tworzenie 16 podkluczy
        /// </summary>
        /// <param name="keyBits"></param>
        /// <returns></returns>
        public List<string> MakeSubkeys(string keyBits)
        {
            var permutatedKey = MakePermutedChoice(keyBits);            // 56-bitowy klucz

            List<string> subKeys = new List<string>();

            for (int i = 0; i < 16; i++)
            {
                var C = permutatedKey.Substring(0, 28);
                var D = permutatedKey.Substring(28, 28);

                string left = shiftLeft(C, tabelaPrzesuniec[i]);
                string right = shiftLeft(D, tabelaPrzesuniec[i]);

                permutatedKey = left + right;
                string subKey = MakePermutedChoice2(permutatedKey);                 // 48-bitowe podklucze
                subKeys.Add(subKey);
            }

            return subKeys;
        }

        /// <summary>
        /// Przesunięcie bitów w lewo zadaną ilość razy
        /// </summary>
        /// <param name="bits">Bity do przesunięcia</param>
        /// <param name="times">Ile razy przesunąć</param>
        /// <returns>Przesunięte bity</returns>
        public string shiftLeft(string bits, int times)
        {
            StringBuilder shiftedBits = new StringBuilder(bits);
            for (int i = 0; i < times; i++)
            {
                var firstBit = shiftedBits[0];
                shiftedBits.Remove(0, 1);
                shiftedBits.Append(firstBit);
            }

            return shiftedBits.ToString();
        }

        /// <summary>
        /// Do rozszerzenia 32-bitowego bloku R w blok 48-bitowy
        /// </summary>
        readonly int[] permutacyjnaTablicaRozszerzenia =
        {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        /// <summary>
        /// Zrobienie permutacji rozszerzenia 32-bitowego bloku R w blok 48-bitowy
        /// </summary>
        /// <param name="bits">Ciąg bitów right/param>
        /// <returns>Rozszerzony ciąg bitów do 48</returns>
        public string MakeRightPermutation(string right)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < permutacyjnaTablicaRozszerzenia.Length; i++)
            {
                result.Append(right[permutacyjnaTablicaRozszerzenia[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Funkcja wykonująca operację XOR na 2 liczbach binarnych
        /// </summary>
        /// <param name="a">Liczba A</param>
        /// <param name="b">Liczba B</param>
        /// <returns>XOR na liczbach A i B</returns>
        private string XOR(string a, string b)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                {
                    result.Append('0');
                }
                else
                {
                    result.Append('1');
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Tablice z których zczytujemy dane dla poszczególnych 6-bitowych fragmentów
        /// </summary>
        readonly int[,,] S =
        {
            {
                { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
            },
            {
                { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
            },
            {
                { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
            },
            {
                { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
            },
            {
                { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
            },
            {
                { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
            },
            {
                { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            },
            {
                { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
            }
        };

        /// <summary>
        /// Dzielenie na 8 fragmentów po 6 bitów (redukcja rozmiaru bloku z 48-bitów na 32-bity)
        /// </summary>
        /// <param name="xor">Ciąg zxorowanych wcześniej 48-bitów bitów</param>
        /// <returns>32-bity zapisane w stringu</returns>
        private string DivideTo6Bits(string xor)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < 8; i++)
            {
                string sixBits = xor.Substring(i * 6, 6);

                string row = "" + sixBits[0] + sixBits[5];
                string col = "" + sixBits[1] + sixBits[2] + sixBits[3] + sixBits[4];
                int rowInt = Convert.ToInt32(row, 2);
                int colInt = Convert.ToInt32(col, 2);

                int value = S[i, rowInt, colInt];

                string bitsValue = DecimalToBinary4Bits(value);
                result.Append(bitsValue);
            }

            return result.ToString();
        }

        /// <summary>
        /// Permutowanie 32-bitów
        /// </summary>
        readonly int[] permutation32Bits =
        {
            16,  7, 20, 21,
            29, 12, 28, 17,
             1, 15, 23, 26,
             5, 18, 31, 10,
             2,  8, 24, 14,
            32, 27,  3,  9,
            19, 13, 30,  6,
            22, 11,  4, 25,
        };

        /// <summary>
        /// Zrobienie permutacji 32-bitowej
        /// </summary>
        /// <param name="bits">Ciąg 32-bitów/param>
        /// <returns>Zpermutowany ciąg 32-bitów</returns>
        public string MakePermutation32Bits(string sixBits)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < permutation32Bits.Length; i++)
            {
                result.Append(sixBits[permutation32Bits[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Ostatnia permutacja
        /// </summary>
        int[] ReverseInitialPermutation =
        {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41,  9, 49, 17, 57, 25
        };

        /// <summary>
        /// Wykonanie ostatniej permutacji
        /// </summary>
        /// <param name="rightleft">Połączone bloki prawy i lewy</param>
        /// <returns>Zpermutowany ciąg 64-bitów</returns>
        private string MakeReverseInitialPermutation(string rightleft)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ReverseInitialPermutation.Length; i++)
            {
                result.Append(rightleft[ReverseInitialPermutation[i] - 1]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Zamiana liczby binarnej na zapis HEX (szesnastkowy)
        /// </summary>
        /// <param name="bin">Liczba w zapisie binarnym</param>
        /// <returns>Liczba w zapisie HEX</returns>
        public string BinToHex(string bin)
        {
            string hex = string.Empty;

            for (int i = 0; i < bin.Length; i += 4)
            {

                var fourBitsToHex = bin.Substring(i, 4);
                hex += Convert.ToInt32(fourBitsToHex, 2).ToString("X").ToLower();
            }

            return hex;
        }

        /// <summary>
        /// Funkcja szyfrująca algorytmem DES (szyfruje tylko 64-bitowy blok) - (nie wykorzystywana)
        /// </summary>
        /// <param name="text">Tekst do zaszyfrowania (podany jako plaintext - normalny tekst)</param>
        /// <param name="key">Klucz (podany jako plaintext - normalny tekst)</param>
        /// <returns>Zaszyfrowany tekst</returns>
        public string Code(string text, string key)
        {
            string codedText = "";
            var bits = StringToBin(text);
            var keyBits = StringToBin(key);

            var permutatedBits = MakeInitialPermutation(bits);
            var L = permutatedBits.Substring(0, 32);
            var R = permutatedBits.Substring(32, 32);

            List<string> subkeys = MakeSubkeys(keyBits);
            for (int i = 0; i < subkeys.Count; i++)
            {
                var permutatedRight = MakeRightPermutation(R);          // 48-bitów
                string xor = XOR(permutatedRight, subkeys[i]);
                string sixBits = DivideTo6Bits(xor);                    // 32-bity
                string permutated32Bits = MakePermutation32Bits(sixBits);
                string tmpR = R;
                R = XOR(permutated32Bits, L);
                L = tmpR;
            }

            string rightleft = R + L;
            codedText = MakeReverseInitialPermutation(rightleft);
            string binToHex = BinToHex(codedText);
            return binToHex;
        }

        /// <summary>
        /// Funkcja szyfrująca algorytmem DES (szyfruje tylko jeden 64-bitowy blok)
        /// </summary>
        /// <param name="text">Tekst do zaszyfrowania (podany w 64 bitach)</param>
        /// <param name="key">Klucz (podany jako plaintext - normalny tekst)</param>
        /// <returns>Zaszyfrowany tekst</returns>
        public string CodeOneBlock(string text, string key)
        {
            string codedText = "";
            var bits = text;
            var keyBits = StringToBin(key);

            var permutatedBits = MakeInitialPermutation(bits);
            var L = permutatedBits.Substring(0, 32);
            var R = permutatedBits.Substring(32, 32);

            List<string> subkeys = MakeSubkeys(keyBits);
            for (int i = 0; i < subkeys.Count; i++)
            {
                var permutatedRight = MakeRightPermutation(R);          // 48-bitów
                string xor = XOR(permutatedRight, subkeys[i]);
                string sixBits = DivideTo6Bits(xor);                    // 32-bity
                string permutated32Bits = MakePermutation32Bits(sixBits);
                string tmpR = R;
                R = XOR(permutated32Bits, L);
                L = tmpR;
            }

            string rightleft = R + L;
            codedText = MakeReverseInitialPermutation(rightleft);
            string binToHex = BinToHex(codedText);
            return binToHex;
        }

        /// <summary>
        /// Funkcja szyfrująca algorytmem DES (szyfruje wiele bloków tekstu)
        /// </summary>
        /// <param name="text">Tekst do zaszyfrowania (plaintext - zwykły tekst)</param>
        /// <param name="key">Klucz (plaintext - zwykły tekst)</param>
        /// <returns>Zaszyfrowany blok tekstu</returns>
        public string CodeBlocks(string text, string key)
        {
            string codedText = "";

            var bits = AddBlocks(text);
            var keyBits = key;

            var blocksNumber = bits.Length / 64;
            for (int i = 0; i < blocksNumber; i++)
            {
                var block = bits.Substring(i * 64, 64);
                var codedBlock = CodeOneBlock(block, keyBits);
                codedText += codedBlock;
            }

            return codedText;
        }

        /// <summary>
        /// Zmana liczby HEX na BIN
        /// </summary>
        /// <param name="hex">Liczba HEX</param>
        /// <returns>Liczba BIN</returns>
        private string HexToBin(string hex)
        {
            var dictionary = new Dictionary<char, string>{
            { '0', "0000"},
            { '1', "0001"},
            { '2', "0010"},
            { '3', "0011"},
            { '4', "0100"},
            { '5', "0101"},
            { '6', "0110"},
            { '7', "0111"},
            { '8', "1000"},
            { '9', "1001"},
            { 'a', "1010"},
            { 'b', "1011"},
            { 'c', "1100"},
            { 'd', "1101"},
            { 'e', "1110"},
            { 'f', "1111"},
            { 'A', "1010"},
            { 'B', "1011"},
            { 'C', "1100"},
            { 'D', "1101"},
            { 'E', "1110"},
            { 'F', "1111"}};

            string result = ""; 
            foreach (var character in hex)
            {
                result += dictionary[character];
            }

            return result;
        }

        /// <summary>
        /// Funkcja deszyfrująca algorytmem DES (deszyfruje wiele bloków tekstu)
        /// </summary>
        /// <param name="text">Tekst do odszyfrowania (podawany w formacie HEX)</param>
        /// <param name="key">Klucz (plaintext - zwykły tekst)</param>
        /// <returns>Odszyfrowany blok tekstu</returns>
        public string DecodeBlocks(string text, string key)
        {
            string decodedText = "";
            var keyBits = key;

            var blocksNumber = text.Length / 16;
            for (int i = 0; i < blocksNumber; i++)
            {
                var block = text.Substring(i * 16, 16);
                string hexToBin = HexToBin(block);
                var decodedBlock = DecodeOneBlock(hexToBin, keyBits);
                decodedText += decodedBlock;
            }

            return decodedText;
        }

        /// <summary>
        /// Funkcja deszyfrująca algorytmem DES (deszyfruje tylko jeden 64-bitowy blok)
        /// </summary>
        /// <param name="text">Tekst do odszyfrowania (podany w 64 bitach)</param>
        /// <param name="key">Klucz (podany jako plaintext - normalny tekst)</param>
        /// <returns>Odszyfrowany tekst</returns>
        private string DecodeOneBlock(string text, string key)
        {
            string codedText = "";
            var bits = text;
            var keyBits = StringToBin(key);

            var permutatedBits = MakeInitialPermutation(bits);
            var L = permutatedBits.Substring(0, 32);
            var R = permutatedBits.Substring(32, 32);

            List<string> subkeys = MakeSubkeys(keyBits);
            for (int i = 15; i >= 0; i--)
            {
                var permutatedRight = MakeRightPermutation(R);          // 48-bitów
                string xor = XOR(permutatedRight, subkeys[i]);
                string sixBits = DivideTo6Bits(xor);                    // 32-bity
                string permutated32Bits = MakePermutation32Bits(sixBits);
                string tmpR = R;
                R = XOR(permutated32Bits, L);
                L = tmpR;
            }

            string rightleft = R + L;
            codedText = MakeReverseInitialPermutation(rightleft);

            string binToHex = BinToHex(codedText);
            string napis = BinToString(codedText).TrimEnd('\0');
            return napis;
        }

        /// <summary>
        /// Funkcja szyfrująca algorytmem DES z pliku binarnego
        /// </summary>
        /// <param name="decodedFilePath">Plik binarny z odszyfrowanym tekstem</param>
        /// <param name="key">Klucz (podany jako plaintext - normalny tekst)</param>
        /// <returns>Zaszyfrowany text</returns>
        public string CodeBlocksFromBinary(string decodedFilePath, string key)
        {
            byte[] fileBytes = File.ReadAllBytes(decodedFilePath);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in fileBytes)
            {
                sb.Append((char)b);
            }
            var text = sb.ToString().Trim('\0');
            var codedBlocks = CodeBlocks(text, key);
            return codedBlocks;
        }

        /// <summary>
        /// Funkcja deszyfrująca algorytmem DES z pliku binarnego
        /// </summary>
        /// <param name="codedFilePath">Plik binarny z zaszyfrowanym tekstem</param>
        /// <param name="key">Klucz (podany jako plaintext - normalny tekst)</param>
        /// <returns>Odszyfrowany text</returns>
        public string DecodeBlocksFromBinary(string codedFilePath, string key)
        {
            byte[] fileBytes = File.ReadAllBytes(codedFilePath);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in fileBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            var codedText = sb.ToString();
            codedText = BinToHex(codedText);
            var decodedBlocks = DecodeBlocks(codedText, key);
            return decodedBlocks;
        }
    }
}
