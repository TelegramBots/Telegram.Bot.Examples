using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class PollingExtensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
    {
    var o = serviceProvider.GetService<IOptions<T>>();
    if (o is null)
        throw new ArgumentNullException(nameof(T));

    return o.Value;
    }
}
