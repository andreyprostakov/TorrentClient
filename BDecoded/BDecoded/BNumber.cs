using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public class BNumber : IBElement
    {
        public long Value { get; set; }


        public BNumber(long value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public String BEncode()
        {
            return String.Format("i{0}e", Value);
        }
    }
}
