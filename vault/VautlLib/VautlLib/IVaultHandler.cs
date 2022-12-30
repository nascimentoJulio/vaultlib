using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VautlLib.Responses;

namespace VautlLib
{
    public interface IVaultHandler
    {
        Task<VaultResponse> GetCredentials(string path, Dictionary<string, string>? headers);

        Task<object> GetCredentialsFromVaultLib(string path, Dictionary<string, string>? headers);

        Task<object> GetCredentialByKey(string key, Dictionary<string, string>? headers);

        Task<object> GetAuditLogs(Dictionary<string, string>? headers);
    }
}
