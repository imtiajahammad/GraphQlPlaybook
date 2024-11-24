using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http.Extensions;

namespace GraphQlDotNetCore;

public class AppQuery : ObjectGraphType
{
    public AppQuery(IOwnerRepository repository)
    {
        Field<ListGraphType<OwnerType>>(
           "owners",
           resolve: context => repository.GetAll()
       );
       Field<OwnerType>(
            "owner",
            arguments: new QueryArguments( new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "ownerId" }),
            resolve: context => 
            {
                /*var id = context.GetArgument<Guid>("ownerId");
                return repository.GetById(id);*/
                Guid id;
                if( !Guid.TryParse(context.GetArgument<string>("ownerId"), out id))
                {
                    context.Errors.Add(new ExecutionError("Wrong value for guid"));
                    return null;
                }
                
                return repository.GetById(id);
            }
       );
    }
}
