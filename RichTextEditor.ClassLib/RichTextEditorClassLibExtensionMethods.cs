using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace RichTextEditor.ClassLib;

public static class RichTextEditorClassLibExtensionMethods
{
    public static IServiceCollection AddRichTextEditorClassLibServices(this IServiceCollection services)
    {
        return services
            .AddFluxor(options => options
                .ScanAssemblies(typeof(RichTextEditorClassLibExtensionMethods).Assembly));
    }
}
