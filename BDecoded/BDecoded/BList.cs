using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public class BList : IBElement
    {
        public List<IBElement> Values { get; set; }

        public IBElement this[int index]
        {
            get
            {
                if (index < 0 || index >= Values.Count) throw new IndexOutOfRangeException();
                return Values[index];
            }
        }

        public int Count
        {
            get { return Values.Count; }
        }

        public IBElement First
        {
            get { return Values.First(); }
        }

        public IBElement Last
        {
            get { return Values.Last(); }
        }


        public BList(List<IBElement> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return String.Format("{0}", String.Join(", ", Values.Select(v => v.ToString())));
        }

        public String BEncode()
        {
            return String.Format("l{0}e", String.Join("", Values.Select(v => v.BEncode())));
        }
    }
}
