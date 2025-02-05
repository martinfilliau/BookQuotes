using BookQuotes.Application.Commands;
using BookQuotes.Infrastructure.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddSingleton<IAnalyseQuotes, AnalyseQuotes>();
        builder.Services.AddSingleton<IAnalyseEpubFile, AnalyseEpubFile>();
    }
}