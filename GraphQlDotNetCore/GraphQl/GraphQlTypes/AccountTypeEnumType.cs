using GraphQL.Types;

namespace GraphQlDotNetCore;

public class AccountTypeEnumType : EnumerationGraphType<TypeOfAccount>
{
    public AccountTypeEnumType()
    {
        Name = "Type";
        Description = "Enumeration for the account type object.";
    }
}