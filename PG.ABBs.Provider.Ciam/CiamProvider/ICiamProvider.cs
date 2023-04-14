using PG.ABBs.Provider.Ciam;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Provider.Ciam.CiamProvider
{
    public interface ICiamProvider
    {

        public string Name { get; set; }

        CiamBase FetchProfile(NameValueCollection collection, Dictionary<string, string> content);

    }
}
