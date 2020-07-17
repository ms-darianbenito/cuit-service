using CuitService.TaxInfoProvider;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Tavis.UriTemplates;

namespace CuitService.Controllers
{
    [Authorize]
    [ApiController]
    public class TaxInfoController
    {
        private readonly ILogger<TaxInfoController> _logger;
        private readonly TaxInfoProviderOptions _taxInfoProviderOptions;

        public TaxInfoController(ILogger<TaxInfoController> logger, IOptions<TaxInfoProviderOptions> taxInfoProviderOptions)
        {
            _logger = logger;
            _taxInfoProviderOptions = taxInfoProviderOptions.Value;
        }

        // TODO: rename cuitNumber parameter as cuit
        [HttpGet("/taxinfo/by-cuit/{cuit}")]
        public async Task<TaxInfo> GetTaxInfoByCuit([FromRoute] CuitNumber cuitNumber)
        {
            var url = new UriTemplate(_taxInfoProviderOptions.UriTemplate)
                .AddParameter("host", _taxInfoProviderOptions.Host)
                .AddParameter("cuit", cuitNumber.SimplifiedValue)
                .Resolve();

            var request = url
                .WithHeader("UserName", _taxInfoProviderOptions.Username)
                .WithHeader("Password", _taxInfoProviderOptions.Password);

            var result = await request.GetJsonAsync<TaxInfo>();

            return result;
        }
    }
}
