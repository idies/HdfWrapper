using HDF.PInvoke;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HdfSql
{
    public class ClrFunctions
    {
        const uint SIZEOF_CHAR = 1;

        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static string GetHdfVersion()
        {
            uint majnum = 0, minnum = 0, relnum = 0;
            H5.get_libversion(ref majnum, ref minnum, ref relnum);
            return majnum + "." + minnum + "." + relnum;
        }

        [SqlFunction(FillRowMethodName = "FillRow")]
        public static IEnumerable ReadStrings(string filename, string dataset)
        {
            var f = H5F.open(filename, H5F.ACC_RDONLY);
            if (f < 0) throw new Exception("Could not open file: " + filename);

            var dset = H5D.open(f, Encoding.ASCII.GetBytes(dataset), H5P.DEFAULT);
            if (dset < 0) throw new Exception("Could not open dataset: " + dataset);

            var filetype = H5D.get_type(dset);
            var sdim = H5T.get_size(filetype) + 1;

            var space = H5D.get_space(dset);
            var ndims = H5S.get_simple_extent_ndims(space);
            ulong[] dims = new ulong[ndims];
            H5S.get_simple_extent_dims(space, dims, null);
            var memtype = H5T.copy(H5T.C_S1);
            var status = H5T.set_size(memtype, sdim);

            int len = (int)(dims[0] * (ulong)sdim * SIZEOF_CHAR);
            byte[] buffer = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);

            status = H5D.read(dset, memtype, H5S.ALL, H5S.ALL, H5P.DEFAULT, ptr);
            Marshal.Copy(ptr, buffer, 0, len);
            Marshal.FreeHGlobal(ptr);

            string s = Encoding.ASCII.GetString(buffer);
            return s.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void FillRow(Object obj, out SqlChars value)
        {
            value = new SqlChars(obj.ToString());
        }
    }
}
