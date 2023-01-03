using Newtonsoft.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VautlLib.Responses;
using VaultSharp.V1.SystemBackend;

namespace VautlLib
{
    public class VaultHandler : IVaultHandler
    {
        private readonly HttpClient _client;
        private static IVaultClient _vaultClient;
        public VaultHandler(HttpClient client)
        {
            _vaultClient = InstanceVaultClient();
            _client=client;
        }

        public async Task<object> GetAuditLogs(Dictionary<string, string>? headers)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetCredentialByKey(string key, Dictionary<string, string>? headers)
        {
            throw new NotImplementedException();
        }

        public async Task<VaultResponse> GetCredentials(string path, Dictionary<string, string>? headers)
        {
            var stringfiedCredentials = await _client.GetStringAsync($"/v1/secret/data/{path}");
            return JsonConvert.DeserializeObject<VaultResponse>(stringfiedCredentials);
        }

        public async Task<object> GetCredentialsFromVaultLib(string path, Dictionary<string, string>? headers)
        {
           return await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path, mountPoint: "secret");
        }

        private IVaultClient InstanceVaultClient()
        {
            if (_vaultClient == null)
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "mytoken");
                VaultClientSettings vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod);
                _vaultClient = new VaultClient(vaultClientSettings);
            }
            return _vaultClient;
        }
    }
}
