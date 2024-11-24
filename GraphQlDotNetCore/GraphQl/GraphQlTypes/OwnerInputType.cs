using GraphQL.Types;

namespace GraphQlDotNetCore;

public class OwnerInputType : InputObjectGraphType
{
    public OwnerInputType()
    {
        Name = "ownerInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StringGraphType>>("address");
    }
}
