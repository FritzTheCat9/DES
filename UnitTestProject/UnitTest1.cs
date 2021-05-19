using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [DataTestMethod]
        [DataRow("IEOFIT#1234", "IEOFIT#1", "c733410ad78788fed621f49205e5027e")]
        [DataRow("SIEMA", "ELUWINA1", "11436c189bd3cdb7")]
        [DataRow("KOCHAM SIECI", "SIECIAKI", "6f09fd18096059c937ad5b0905373327")]
        [DataRow("a", "SIECIAKI", "9873086abe39c1ef")]
        [DataRow("TAJNA ZAKODOWANA WIADOMOSC", "ALA12345", "6f70fe29cd898f05f823bf127c50e725619458759d1e5d7b48dc8912a9423d1c")]
        [DataRow("SIECI SIECIOWE", "12345678", "f36338085740ca493b6233f5c6e64ee4")]
        public void DESCodeTests(string text, string key, string expected)
        {
            DES.DES des = new DES.DES();
            var actual = des.CodeBlocks(text, key);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("c733410ad78788fed621f49205e5027e", "IEOFIT#1", "IEOFIT#1234")]
        [DataRow("11436c189bd3cdb7", "ELUWINA1", "SIEMA")]
        [DataRow("6f09fd18096059c937ad5b0905373327", "SIECIAKI", "KOCHAM SIECI")]
        [DataRow("9873086abe39c1ef", "SIECIAKI", "a")]
        [DataRow("6f70fe29cd898f05f823bf127c50e725619458759d1e5d7b48dc8912a9423d1c", "ALA12345", "TAJNA ZAKODOWANA WIADOMOSC")]
        [DataRow("f36338085740ca493b6233f5c6e64ee4", "12345678", "SIECI SIECIOWE")]
        public void DESDecodeTests(string text, string key, string expected)
        {
            DES.DES des = new DES.DES();
            var actual = des.DecodeBlocks(text, key);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("decoded.dat", "IEOFIT#1", "c733410ad78788fed621f49205e5027e")]
        [DataRow("decoded2.dat", "12345678", "f36338085740ca493b6233f5c6e64ee4")]
        public void CodeBlocksFromBinaryTests(string decodedFilePath, string key, string expected)
        {
            DES.DES des = new DES.DES();
            var actual = des.CodeBlocksFromBinary(decodedFilePath, key);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("coded.dat", "IEOFIT#1", "IEOFIT#1234")]
        [DataRow("coded2.dat", "12345678", "SIECI SIECIOWE")]
        public void DecodeBlocksFromBinaryTests(string codedFilePath, string key, string expected)
        {
            DES.DES des = new DES.DES();
            var actual = des.DecodeBlocksFromBinary(codedFilePath, key);
            Assert.AreEqual(expected, actual);
        }
    }
}
