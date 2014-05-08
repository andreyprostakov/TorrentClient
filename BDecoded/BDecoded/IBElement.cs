using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDecoded
{
    public interface IBElement
    {
        String ToString();

        String BEncode();
    }
}
