using HDF.PInvoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HdfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "h5ex_t_string.h5";
            string dataset = "DS1";

            IEnumerable strings = HdfSql.ClrFunctions.ReadStrings(filename, dataset);
            foreach (string str in strings)
            {
                Console.WriteLine(str);
            }
        }
    }
}
