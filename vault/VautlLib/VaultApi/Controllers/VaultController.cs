using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VautlLib;

namespace VaultApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultController : ControllerBase
    {
        private readonly IVaultHandler _vaultHandler;

        public VaultController(IVaultHandler vaultHandler)
        {
            _vaultHandler=vaultHandler;
        }

        [HttpGet("/{path}")]
        public async Task<IActionResult> GetCredentials(string path)
        {
            var credentials = await _vaultHandler.GetCredentials(path,null);
            return Ok(credentials.Data.Data);
        }

        [HttpGet("/lib/{path}")]
        public async Task<IActionResult> GetCredentialsFromVaultLib(string path)
        {
            var credentials = await _vaultHandler.GetCredentialsFromVaultLib(path, null);
            return Ok(credentials);
        }


        [HttpGet("/audit")]
        public async Task<IActionResult> GetAudit()
        {
            var logs = await _vaultHandler.GetAuditLogs(null);
            return Ok(logs);
        }
    }
}
