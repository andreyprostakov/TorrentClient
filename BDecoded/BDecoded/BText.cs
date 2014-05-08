using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public class BText : IBElement
    {
        public String Value { get; set; }


        public BText(String value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public String BEncode()
        {
            return String.Format("{0}:{1}", Value.Length, Value);
        }
    }
}
