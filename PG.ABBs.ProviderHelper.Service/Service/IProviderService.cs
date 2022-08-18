using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.ProviderHelper.Service
{
    public interface IProviderService
    {
        bool VerifyProfile(string encryptionV2Key,
            string ivvar,
            string userId,
            string accessToken,
            string locale);
    }
}
