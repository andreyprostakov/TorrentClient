using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public class BDictionary : IBElement
    {
        public List<BText> Keys { get; set; }

        public List<IBElement> Values { get; set; }

        public IBElement this[String key]
        {
            get
            {
                int index = Keys.FindIndex(k => k.Value == key);
                if (index < 0) return null;
                else return Values[index];
            }
        }

        public int Count
        {
            get { return Keys.Count; }
        }


        public BDictionary(List<BText> keys, List<IBElement> values)
        {
            Keys = keys;
            Values = values;
        }

        public override string ToString()
        {
            List<String> results = new List<string>();
            for (int i=0;i<Keys.Count;i++)
            {
                results.Add(String.Format("{0}:{1}", Keys[i], Values[i]));
            }
            return String.Format("{{{0}}}", String.Join(", \n", results));
        }

        public String BEncode()
        {
            String result = "";
            for (int i = 0; i < Keys.Count; i++)
                result += String.Format("{0}{1}", Keys[i].BEncode(), Values[i].BEncode());
            return String.Format("d{0}e", result);
        }
    }
}
