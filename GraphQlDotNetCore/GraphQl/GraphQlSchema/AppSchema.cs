using GraphQL.Types;

namespace GraphQlDotNetCore;

public class AppSchema : Schema
{
    public AppSchema(IServiceProvider provider) : base(provider)
    {
        Query = provider.GetRequiredService<AppQuery>();
        Mutation = provider.GetRequiredService<AppMutation>();
    }
}
