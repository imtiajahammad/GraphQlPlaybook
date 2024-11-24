using GraphQL.DataLoader;
using GraphQL.Types;

namespace GraphQlDotNetCore;

public class OwnerType : ObjectGraphType<Owner>
{
    public OwnerType(IAccountRepository repository, IDataLoaderContextAccessor dataLoader)
    {
        Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
        Field(x => x.Name).Description("Name property from the owner object.");
        Field(x => x.Address).Description("Address property from the owner object.");
        Field<ListGraphType<AccountType>>(
            "accounts",
            resolve: context => 
            {
                //repository.GetAllAccountsPerOwner(context.Source.Id)
                var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Account>("GetAccountsByOwnerIds", repository.GetAccountsByOwnerIds);
                return loader.LoadAsync(context.Source.Id);
            }
        );
    }
}
