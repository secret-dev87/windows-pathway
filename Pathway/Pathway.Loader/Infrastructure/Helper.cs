using System;
using System.Text;

namespace Pathway.Loader.Infrastructure {
    internal class Helper {
        internal static short Reverse(short tempShort) {
            //This function will reverse the byte and get the right value.
            var nShort = new byte[2];

            byte[] bShort = BitConverter.GetBytes(tempShort);
            nShort[0] = bShort[1];
            nShort[1] = bShort[0];
            short returnShort = Convert.ToInt16(BitConverter.ToInt16(nShort, 0));

            return returnShort;
        }

        internal static int Reverse(int tempInteger) {
            //This function will reverse the byte and get the right value.
            var nInteger = new byte[4];

            byte[] bInteger = BitConverter.GetBytes(tempInteger);
            nInteger[0] = bInteger[3];
            nInteger[1] = bInteger[2];
            nInteger[2] = bInteger[1];
            nInteger[3] = bInteger[0];
            int returnInt = Convert.ToInt32(BitConverter.ToInt32(nInteger, 0));

            return returnInt;
        }

        internal static long Reverse(long tempLong) {
            //This function will reverse the byte and get the right value.
            var nLong = new byte[8];

            byte[] bLong = BitConverter.GetBytes(tempLong);
            nLong[0] = bLong[7];
            nLong[1] = bLong[6];
            nLong[2] = bLong[5];
            nLong[3] = bLong[4];
            nLong[4] = bLong[3];
            nLong[5] = bLong[2];
            nLong[6] = bLong[1];
            nLong[7] = bLong[0];
            long returnLong = Convert.ToInt64(BitConverter.ToInt64(nLong, 0));

            return returnLong;
        }

    }
}