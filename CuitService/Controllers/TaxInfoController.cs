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
    public class TaxInfoController
    {
        private readonly ILogger<TaxInfoController> _logger;
        private readonly TaxInfoProviderOptions _taxInfoProviderOptions;

        public TaxInfoController(ILogger<TaxInfoController> logger, IOptions<TaxInfoProviderOptions> taxInfoProviderOptions)
        {
            _logger = logger;
            _taxInfoProviderOptions = taxInfoProviderOptions.Value;
        }

        // TODO: validate CUIT before, in filter, binder, etc. And avoid
        // primitive obsession in cuit parameter.
        [HttpGet("/taxinfo/by-cuit/{cuit}")]
        public async Task<TaxInfo> GetTaxInfoByCuit(string cuit)
        {
            var url = new UriTemplate(_taxInfoProviderOptions.UriTemplate)
                .AddParameter("host", _taxInfoProviderOptions.Host)
                .AddParameter("cuit", cuit.Replace("-", ""))
                .Resolve();

            var result = await url
                .WithHeader("UserName", _taxInfoProviderOptions.Username)
                .WithHeader("Password", _taxInfoProviderOptions.Password)
                .GetJsonAsync<TaxInfo>();

            return result;
        }
    }
}
