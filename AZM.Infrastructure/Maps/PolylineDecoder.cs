using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public static class PolylineDecoder
    {
        public static List<GeoLocation> Decode(string encoded)
        {
            var result = new List<GeoLocation>();
            int index = 0, lat = 0, lng = 0;

            while (index < encoded.Length)
            {
                lat += DecodeChunk(encoded, ref index);
                lng += DecodeChunk(encoded, ref index);
                result.Add(new GeoLocation(lat / 1e5, lng / 1e5));
            }

            return result;
        }

        private static int DecodeChunk(string encoded, ref int index)
        {
            int result = 0, shift = 0, b;
            do
            {
                b = encoded[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            } while (b >= 0x20);

            return (result & 1) != 0 ? ~(result >> 1) : result >> 1;
        }
    }
}
