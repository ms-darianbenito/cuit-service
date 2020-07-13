using Microsoft.AspNetCore.Mvc.Testing;

namespace CuitService.Test
{
    public static class WebApplicationFactoryHelper
    {
        public static WebApplicationFactory<Startup> Create()
            => new WebApplicationFactory<Startup>();
    }
}
