using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Microphone;

namespace ServiceA.Controllers
{
    public class RemoteController : Controller
    {
        [HttpGet("api/[controller]")]
        public async Task<string> Get([FromServices]IClusterClient client)
        {
            var uri = await client.ResolveUriAsync("ServiceB", "/api/hello");
            var hc = new HttpClient();
            
            return $"Response from remote server({uri}): " + await hc.GetStringAsync(uri);
        }
    }
}