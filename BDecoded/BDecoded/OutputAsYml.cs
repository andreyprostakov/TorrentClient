using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDecoded;

namespace BDecoded
{
    public class OutputAsYml
    {
        const String TAB_QUOTA = "   ";

        public String Output(IBElement element, int tab = 0)
        {
            String tab_string = String.Join("", Enumerable.Repeat(TAB_QUOTA, tab).ToArray());
            if (element is BDictionary)
                return OutputDictionaty((BDictionary)element, tab);
            else if (element is BList)
                return OutputList((BList)element, tab);
            else
                return tab_string + element.ToString();
        }

        protected String OutputList(BList list, int tab)
        {
            int new_tab = tab;
            return String.Join(",\n", list.Values.Select(v => Output(v, new_tab)));
        }

        protected String OutputDictionaty(BDictionary dictionary, int tab)
        {
            List<String> pairs = new List<string>();
            for (int i = 0; i < dictionary.Count; i++)
            {
                if (dictionary.Values[i] is BNumber || dictionary.Values[i] is BText)
                    pairs.Add(String.Format("{0}: {1}", Output(dictionary.Keys[i], tab), dictionary.Values[i]));
                else
                    pairs.Add(String.Format("{0}:\n{1}", Output(dictionary.Keys[i], tab), Output(dictionary.Values[i], tab + 1)));
            }
            return String.Join(",\n", pairs);
        }
    }
}
