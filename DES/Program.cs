using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    class Program
    {
        static void Main(string[] args)
        {
            DES des = new DES();

            /*Console.WriteLine(des.CharToASCII('A'));
            Console.WriteLine(des.DecToBin(65));
            Console.WriteLine(des.StringToBin("SIEMA"));
            Console.WriteLine(des.AddBlocks("12345678"));*/

            /*var s = "0011000100110010001100110011010000110101001101100011011100111000";
            var initialPerm = des.MakeInitialPermutation(s);
            Console.WriteLine(initialPerm);*/

            //Console.WriteLine(des.Code("IEOFIT#1", "IEOFIT#1"));                    // c7	33	41	0a	d7	87	88	fe
            //Console.WriteLine(des.CodeBlocks("IEOFIT#12", "IEOFIT#1"));             // c7	33	41	0a	d7	87	88	fe	08	9a	4f	f4	ac	9a	25	09


            Console.WriteLine(des.CodeBlocks("IEOFIT#1234", "IEOFIT#1"));                                       // c7	33	41	0a	d7	87	88	fe	d6	21	f4	92	05	e5	02	7e
            Console.WriteLine(des.DecodeBlocks("c733410ad78788fed621f49205e5027e", "IEOFIT#1"));                // IEOFIT#1 234                  49454f4649542331 3233340000000000
            Console.ReadKey();
        }
    }
}
