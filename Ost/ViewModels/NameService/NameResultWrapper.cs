using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Programs.Models.NameService;

namespace Ost.ViewModels.NameService
{
    public class NameResultWrapper
    {
        public string Name { get; private set; }

        public string Type { get; private set; }


        public NameResultWrapper(RecordBase baseName)
        {
            if(baseName is ReverseNameRecord rn)
            {
                Name = rn.Name;
                Type = "Sol Name";
            }
            else if ( baseName is ReverseTokenNameRecord rtn)
            {
                Name = rtn.Value.Key;
                Type = "Token Mint";
            }
            else if (baseName is ReverseTwitterRecord rt)
            {
                Name = rt.TwitterHandle;
                Type = "Twitter";
            }
        }
    }
}
