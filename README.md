# GraphQlPlaybook
A graphQl step by step implementation with Asp.net core.

## Getting Started
Instructions on how to get a copy of the project up and running on your local machine.

### Prerequisites
List any software, libraries, or hardware needed to run this project.
 Lists : 
 - net8.0
 - Visual Studio Code

## Implementation Stepdown
1.  Open ***cmd*** and go to the your preferred directory
    ```
    mkdir GraphQlPlaybook
    ```
2. Create new solution 
    ```
    dotnet new sln -n GraphQlPlaybook
    ``` 
3. I am using ***VisualStudioCode*** for the editor, so will open the solution with ***vsCode***
    ```
    code .
    ```
4. Create a ***gitignore*** file in the solution
    ```
    dotnet new gitignore
    ``` 
5. Create a ***readme.md*** file
    ```
    touch README.md 
    ```
6. Create a ***webapi*** project in the solution
    ```
    dotnet new webapi -f net8.0 -n GraphQlDotNetCore
    ```
7. Add the ***project*** into the ***solution***
    ```
    dotnet sln add GraphQlDotNetCore/GraphQlDotNetCore.csproj
    ```
8. Add following packages into the ***GraphQlDotNetCore*** 
    
    ```
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```
9. Create a new folder named ***Contracts*** into the project folder and add ***interfaces*** into the folder
    ```
    mkdir Contracts
    cd Contracts
    ```
    ```
    dotnet new class -n IAccountRepository
    ```
    ```
    namespace GraphQlDotNetCore;

    public interface IAccountRepository
    {
        
    }
    ```
    ```
    dotnet new class -n IOwnerRepository
    ```
    ```
    namespace GraphQlDotNetCore;

    public interface IOwnerRepository
    {

    }
    ```
10. Create a new folder ***Entities*** and add ***Entity*** classes and ***dbContext*** into it
    ```
    mkdir Entities
    cd Entities
    ```
    ```
    dotnet new class -n Owner
    ```
    ```
    using System.ComponentModel.DataAnnotations;

    namespace GraphQlDotNetCore;

    public class Owner
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
    ```
    ```
    dotnet new class -n Account
    ```
    ```
    using System.ComponentModel.DataAnnotations;

    namespace GraphQlDotNetCore;

    public class Account
    {
            [Key]
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Type is required")]
            public TypeOfAccount Type { get; set; }
            public string Description { get; set; }

            [ForeignKey("OwnerId")]
            public Guid OwnerId { get; set; }
            public Owner Owner { get; set; }
    }
    ```
    ```
    dotnet new class -n TypeOfAccount
    ```
    ```
    namespace GraphQlDotNetCore;

    public enum TypeOfAccount
    {
        Cash,
        Savings,
        Expense,
        Income
    }
    ```
    ```
    mkdir Context
    cd Context
    dotnet new class -n ApplicationContext 
    ```
    ```
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    namespace GraphQlDotNetCore;

    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var ids = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

            modelBuilder.ApplyConfiguration(new OwnerContextConfiguration(ids));
            modelBuilder.ApplyConfiguration(new AccountContextConfiguration(ids));
        }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Account> Accounts { get; set; }

    }
    ```
    ```
    dotnet new class -n AccountContextConfiguration
    ```
    ```
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace GraphQlDotNetCore;

    public class AccountContextConfiguration : IEntityTypeConfiguration<Account>
    {
        private Guid[] _ids;

        public AccountContextConfiguration(Guid[] ids)
        {
            _ids = ids;
        }

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder
                .HasData(
                new Account
                {
                    Id = Guid.NewGuid(),
                    Type = TypeOfAccount.Cash,
                    Description = "Cash account for our users",
                    OwnerId = _ids[0]
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Type = TypeOfAccount.Savings,
                    Description = "Savings account for our users",
                    OwnerId = _ids[1]
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    Type = TypeOfAccount.Income,
                    Description = "Income account for our users",
                    OwnerId = _ids[1]
                }
        );
        }
    }
    ```
    ```
    dotnet new class -n OwnerContextConfiguration
    ```
    ```
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace GraphQlDotNetCore;

    public class OwnerContextConfiguration : IEntityTypeConfiguration<Owner>
    {
        private Guid[] _ids;

        public OwnerContextConfiguration(Guid[] ids)
        {
            _ids = ids;
        }

        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder
            .HasData(
                new Owner
                {
                    Id = _ids[0],
                    Name = "John Doe",
                    Address = "John Doe's address"
                },
                new Owner
                {
                    Id = _ids[1],
                    Name = "Jane Doe",
                    Address = "Jane Doe's address"
                }
            );
        }
    }
    ```
11. Create a folder named ***Repository*** and add the following ***classes***
    ```
    mkdir Repository
    cd Repository
    ```
    ```
    dotnet new class -n OwnerRepository
    ```
    ```
    namespace GraphQlDotNetCore;

    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext _context;

        public AccountRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
    ```
    ```
    dotnet new class -n AccountRepository
    ```
    ```
    namespace GraphQlDotNetCore;

    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
    ```
12. Register ***context*** class and ***repository*** classes inside the ***Program.cs*** class
    ```
    builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConString")));

    builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    ```
13. Add ***connectionStrings*** for ***db*** in the ***appsettings***
    ```
    {
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "sqlConString": "server=DESKTOP-HUNN46V\\SQLEXPRESS;Database=GraphQLDotNetCore;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    }
    ```
14. Now its time for ***Migrations***
    ```
    dotnet ef add migrations initial_migration
    dotnet ef database update
    ```
15. Let's integrate ***GraphQl*** into the project. Add the ***GraphQl*** packages into the project
    ```
    dotnet add package GraphQl
    dotnet add package GraphQL.Server.Transports.AspNetCore
    dotnet add package GraphQL.Server.Transports.AspNetCore.SystemTextJson
    dotnet add package GraphQL.Server.Ui.Playground
    ```
16. Create a new Folder ***GraphQl.GraphQlSchema*** and add schema
    ```
    mkdir GraphQl
    cd GraphQl
    mkdir GraphQlSchema
    cd GraphQlSchema
    dotnet new class -n AppSchema
    ```
    ```
    using GraphQL.Types;

    namespace GraphQlDotNetCore;

    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider) : base(provider)
        {
            
        }
    }
    ```
    This class must inherit from the Schema class which resides in the GraphQL.Types namespace. 
    Inside the constructor, we inject the IServiceProvider which is going to help us provide our Query, Mutation, or Subscription objects.
    What’s important to know is that each of the schema properties (Query, Mutation, or Subscription) implements IObjectGraphType which means that the objects we are going to resolve must implement the same type as well. 
    This also means that our GraphQL API can’t return our models directly as a result but GraphQL types that implement IObjectGraphType instead.
17. Create a new folder ***GraphQlTypes*** into ***GraphQl*** folder and add new class into it
    ```
    mkdir GraphQlTypes
    cd GraphQlTypes
    dotnet new class -n GraphQlTypes
    ```
    ```
    using GraphQL.Types;

    namespace GraphQlDotNetCore;

    public class OwnerType : ObjectGraphType<Owner>
    {
        public OwnerType()
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.Name).Description("Name property from the owner object.");
            Field(x => x.Address).Description("Address property from the owner object.");
        }
    }
    ```
    This is the OwnerType class which we use as a replacement for the Owner model inside a GraphQL API. 
    This class inherits from a generic ObjectGraphType<Owner> class which at some point (in the hierarchy) implements IObjectGraphType interface. 
    With the Field method, we specify the fields which represent our properties from the Owner model class.
18. Create a new folder ***GraphQlQueries*** into ***GraphQl*** folder and add query class into it
    ```
    mkdir GraphQlQueries
    cd GraphQlQueries
    dotnet new class -n AppQuery
    ```
    ```
    using GraphQL.Types;

    namespace GraphQlDotNetCore;

    public class AppQuery : ObjectGraphType
    {
        public AppQuery(IOwnerRepository repository)
        {
            Field<ListGraphType<OwnerType>>(
            "owners",
            resolve: context => repository.GetAll()
        );
        }
    }
    ```
    As we can see, this class inherits from the ObjectGraphType as well, just the non-generic one. Moreover, we inject our repository object inside a constructor and create a field to return the result for the specific query.
    In this class, we use the generic version of the Field method which accepts some „strange“ type as a generic parameter. Well, this is the GraphQL.NET representation for the normal .NET types. So, ListGraphType is the representation of the List type, and of course, we have IntGraphType or StringGraphType, etc… For the complete list visit SchemaTypes in GraphQL .NET. 
    The „owners“ parameter is a field name (query from the client must match this name) and the second parameter is the result itself.

19. Now, modify the ***interface*** and its implementation for ***IOwnerRepository***
    ```
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetAll();
    }
    ```
    ```
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Owner> GetAll() => _context.Owners.ToList();
    }
    ```
20. Now let's modify the ***AppSchema***
    ```
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<AppQuery>();
        }
    }
    ```
21. We will register the schema in the ***program*** class
    ```
    using GraphQL;
    using GraphQL.Server;
    using GraphQL.Server.Ui.Playground;
    using GraphQlDotNetCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConString")));

    builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();

    builder.Services.AddScoped<AppSchema>();

    builder.Services.AddGraphQL()
            .AddSystemTextJson()
            .AddGraphTypes(typeof(AppSchema), ServiceLifetime.Scoped);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseGraphQL<AppSchema>();
    app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

    app.Run();

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    ```
    update the libraries if you do not find the references
    ```
    <PackageReference Include="GraphQl" Version="3.1.5" />
    <PackageReference Include="GraphQL.Server.Transports.AspNetCore" Version="4.3.1" />
    <PackageReference Include="GraphQL.Server.Transports.AspNetCore.SystemTextJson" Version="4.3.1" />
    <PackageReference Include="GraphQL.Server.Ui.Playground" Version="4.3.1" />
    ```
22. Now try and test the application
    ```
    http://localhost:5217/ui/playground
    ```
    Reference till this stage: 
    - https://code-maze.com/graphql-aspnetcore-basics/
    - https://github.com/CodeMazeBlog/graphql-series/tree/getting-started-with-graphql    

23. Go to ***GraphQl->GraphQlTypes*** folder, add new class
    ```
    dotnet new class -n AccountType
    dotnet new class -n AccountTypeEnumType
    ```
    ```
    public class AccountType : ObjectGraphType<Account>
    {
        public AccountType()
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the account object.");
            Field(x => x.Description).Description("Description property from the account object.");
            Field(x => x.OwnerId, type: typeof(IdGraphType)).Description("OwnerId property from the account object.");
            Field<AccountTypeEnumType>("Type", "Enumeration for the account type object.");
        }
    }
    ```
    If we take a look at our „owners“ query, we are going to see it returns the ListGraphType<OwnerType> result. Furthermore, if we inspect the OwnerType class, we are going to see it contains the Id, Name and Address fields. But a single Owner can have multiple accounts related to it, this can be confirmed by inspecting the Owner model class in the Entities folder. So, this is exactly what we want to add to the OwnerType class as well.
    But, before we start with adding additional Accounts field into the OwnerType class, we need to create the AccountType class first. 
    ```
    public class AccountTypeEnumType : EnumerationGraphType<TypeOfAccount>
    {
        public AccountTypeEnumType()
        {
            Name = "Type";
            Description = "Enumeration for the account type object.";
        }
    }
    ```
    In the AccountType class, we are missing the Type field. We left it out deliberately, and now it’s the right time to add it. To add enumeration to the AccountType class, we need to invest just a little bit more effort than with the regular scalar types in GraphQL.
    The first thing we are going to do is to create a new class AccountTypeEnumType.

    We can see that the AccountTypeEnumType class must inherit from the generic EnumerationGraphType which for the generic parameter has the enumeration that we have created in our starter project. 
    One important thing to mention is that the value for the Name property must match the name of the same enumeration property inside the Account class.

24. Modify the interface ***Contracts->IAccountRepository***
    ```
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccountsPerOwner(Guid ownerId);
    }
    ```
25. Modify the interface implementation ***Repository->AccountRepository***
    ```
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext _context;
        public AccountRepository(ApplicationContext context)
        {
        _context = context;
        }
        public IEnumerable<Account> GetAllAccountsPerOwner(Guid ownerId) => _context.Accounts
            .Where(a => a.OwnerId.Equals(ownerId))
            .ToList();
    }
    ```
26. Modify the ***GraphQl->GraphQlTypes->OwnerType***
    ```
    public class OwnerType : ObjectGraphType<Owner>
    {
        public OwnerType(IAccountRepository repository)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.Name).Description("Name property from the owner object.");
            Field(x => x.Address).Description("Address property from the owner object.");
            Field<ListGraphType<AccountType>>(
                "accounts",
                resolve: context => repository.GetAllAccountsPerOwner(context.Source.Id)
            );
        }
    }
    ```
27. Now run the project, explore the ***graphQl*** url(http://localhost:5217/graphql) and check the accounts data and its ***enum*** types by updating the query.

28. Go to ***Contracts->IAccountRepository*** and modify the interface with new method
    ```
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccountsPerOwner(Guid ownerId);
        Task<ILookup<Guid, Account>> GetAccountsByOwnerIds(IEnumerable<Guid> ownerIds);
    }
    ```
29. Now implement the new method into ***Repository-> AccountRepository***
    ```
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext _context;

        public AccountRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Account> GetAllAccountsPerOwner(Guid ownerId) => _context.Accounts
                .Where(a => a.OwnerId.Equals(ownerId))
                .ToList();
        public async Task<ILookup<Guid, Account>> GetAccountsBYOwnerIds(IEnumerable<Guid> ownerIds)
        {
            var accounts = await _context.Accounts.Where(a => ownerIds.Contains(a.OwnerId)).ToListAsync();
            return accounts.ToLookup(x => x.OwnerId);
        }
    }
    ```
    We need to have a method that returns Task<ILookup<TKey, T> because DataLoader requires a method with that return type in its signature.
    Then, we need to implement this additional method inside the AccountRepository class:
30. Go to ***GraphQl-> GraphQlTypes-> OwnerType*** and modify 
    ```
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
    ```
31. Now register the ***DataLoader*** into the ***program.cs*** file
    ```
    builder.Services.AddGraphQL()
            .AddSystemTextJson()
            .AddGraphTypes(typeof(AppSchema), ServiceLifetime.Scoped)
            .AddDataLoader();
    ```
    Our query is working in such a way that it extracts id’s from all the owners and then for every single id sends the additional SQL query to the database to fetch related accounts. We can see that from the logs.
    Of course, this is not a problem when we have only two owner entities, but what if we have a thousand?
    We can optimize this query by using DataLoader which is provided by GraphQL, with a couple of modifications
32. Now, if we run the application and execute different queries, we can observe that we have only one query for all the accounts per each owner.
33. Let's go to ***Contracts-> IOwnerRepository*** and add new method with argument. Also Add the implementation
    ```
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetAll();
        Owner GetById(Guid id);
    }
    ```
    ```
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Owner> GetAll() => _context.Owners.ToList();
        public Owner GetById(Guid id) => _context.Owners.SingleOrDefault(o => o.Id.Equals(id));
    }
    ```
34. Now modify the ***GraphQl->GraphQlQueries->AppQuery*** with the arguments
    ```
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
    ```
    we create a new field with the OwnerType return value. The name is „owner“ and we use the arguments part to create an argument for this query. Our argument can’t be null (NonNullGraphType) and it must be of the IdGraphType type with the „ownerId“ name. The resolve part is pretty self-explanatory.
    But what if the id parameter is not of the Guid type, then, we would like to return a message to the client. So let’s add a slight modification in the resolve part
35. After these changes, we can use our ***UI*** tool to send a new query with argument
    ```
    query Owners {
        owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            accounts {
                id
                ownerId
            }
            id
        }
    }
    ```
- ***Aliases***:

    ```
    query Owners {
        first:owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            accounts {
                id
                ownerId
            }
            id
        }
    }
    ```
    ```
    query Owners {
        first:owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            accounts {
                id
                ownerId
            }
            id
        },
        second:owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            accounts {
                id
                ownerId
            }
            id
        }
    }
    ```
- ***Fragments***:

    ```
    query Owners {
        first:owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            ...ownerFields
        },
        second:owner(ownerId: "5e6455ff-12c0-4b6b-80f8-e2ff35d68151") {
            ...ownerFields
        }
    }

    fragment  ownerFields on OwnerType{
        id
        name
        address
        accounts{
            id
            type
        }
    }
    ```
- ***Variables***
    ```
    query ownerQuery($ownerID: ID!){
        owner(ownerId: $ownerID){
            id,
            name,
            address,
            accounts{
                id,
                type
            }
        }
    }
    {
    "ownerID": "5e6455ff-12c0-4b6b-80f8-e2ff35d68151"
    }
    ```
- ***directives***
    ```
    query ownerQuery($ownerID: ID!, $showName: Boolean!){
    owner(ownerId : $ownerID){
        id
        name @include(if: $showName)
        address
        accounts{
        id
        type
        }
    }
    }
    {
    "ownerID": "5e6455ff-12c0-4b6b-80f8-e2ff35d68151",
    "showName": false
    }
    ```
    Reference till this stage: 
    - https://code-maze.com/advanced-graphql-queries/
    - https://github.com/CodeMazeBlog/graphql-series/tree/graphql-queries

36. Lets create a new class ***OwnerInputType*** for graphql mutations in ***GraphQl->GraphQlTypes***
    ```
    dotnet new class -n OwnerInputType
    ```
    ```
    public class OwnerInputType : InputObjectGraphType
    {
        public OwnerInputType()
        {
            Name = "ownerInput"
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("address");
        }
    }
    ```
37. Add ***AppMutation*** class into the ***GraphQl->GraphQlQueries***
    ```
    dotnet new class -n AppMutation
    ```
    ```
    public class AppMutation : InputObjectGraphType
    {
        public AppMutation()
        {
            
        }
    }
    ```
38. Now we need to update the ***Schema*** class in the ***GraphQl->GraphQlSchema->AppSchema***
    ```
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<AppQuery>();
            Mutation = provider.GetRequiredService<AppMutation>();
        }
    }
    ```
39. Now let's go to Contracts and update the ***IOwnerRepository*** interface and add mutation method. Also add the implementation also.
    ```
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetAll();
        Owner GetById(Guid id);
        Owner CreateOwner(Owner owner);
    }
    ```
    ```
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Owner> GetAll() => _context.Owners.ToList();
        public Owner GetById(Guid id) => _context.Owners.SingleOrDefault(o => o.Id.Equals(id));
        public Owner CreateOwner(Owner owner)
        {
            owner.Id = Guid.NewGuid();
            _context.Add(owner);
            _context.SaveChanges();
            return owner;
        }
    }
    ```
40. Now let's update the ***AppMutation*** accordingly
    ```
    public AppMutation(IOwnerRepository repository)
    {
        Field<OwnerType>(
            "createOwner",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OwnerInputType>> { Name = "owner"}),
            resolve: context =>
            {
                var owner = context.GetArgument<Owner>("owner");
                return repository.CreateOwner(owner);
            }
        );
    }
    ```
41. Now let run the project and try out the insert mutation
    ```
    mutation($owner: ownerInput!){
    createOwner(owner: $owner){
        id,
        name,
        address
    }
    }
    {
    "owner": {
        "name" : "new name",
        "address" : "new address"
    }
    }
    ```
42. Let's go to ***IOwnerRepository*** and update it with update method. Also add its implementation
    ```
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetAll();
        Owner GetById(Guid id);
        Owner CreateOwner(Owner owner);
        Owner UpdateOwner(Owner dbOwner, Owner owner);
    }
    ```
    ```
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Owner> GetAll() => _context.Owners.ToList();
        public Owner GetById(Guid id) => _context.Owners.SingleOrDefault(o => o.Id.Equals(id));
        public Owner CreateOwner(Owner owner)
        {
            owner.Id = Guid.NewGuid();
            _context.Add(owner);
            _context.SaveChanges();
            return owner;
        }
        public Owner UpdateOwner(Owner dbOwner, Owner owner)
        {
            dbOwner.Name = owner.Name;
            dbOwner.Address = owner.Address;

            _context.SaveChanges();

            return dbOwner;
        }
    } 
    ```
43. We have to add an addtional field in a constructor of the ***AppMutation*** class
    ```
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(IOwnerRepository repository)
        {
            Field<OwnerType>(
                "createOwner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OwnerInputType>> { Name = "owner"}),
                resolve: context =>
                {
                    var owner = context.GetArgument<Owner>("owner");
                    return repository.CreateOwner(owner);
                }
            );
            Field<OwnerType>(
                "updateOwner",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<OwnerInputType>> { Name = "owner"},
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "ownerId" }),
                resolve: context => 
                {
                    var owner = context.GetArgument<Owner>("owner");
                    var ownerId = context.GetArgument<Guid>("ownerId");

                    var dbOwner = repository.GetById(ownerId);
                    if(dbOwner == null)
                    {
                        context.Errors.Add(new ExecutionError("Couldn't find owner in db"));
                        return null;
                    }

                    return repository.UpdateOwner(dbOwner, owner);
                }
            );
        }
    }
    ```
44. Now let's run the project and test it
    ```
    mutation($owner: ownerInput!, $ownerId: ID!){
    updateOwner(owner:$owner, ownerId: $ownerId){
        id,
        name,
        address
    }
    }
    {
    "owner":{
        "name":"updated name",
        "address": "updated address"
    },
    "ownerId": "c4ed9f1c-f112-4bd8-8d9f-3cfb7ee50e57"
    }
    ```
45. Now let's do the same for ***Delete***
    ```
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetAll();
        Owner GetById(Guid id);
        Owner CreateOwner(Owner owner);
        Owner UpdateOwner(Owner dbOwner, Owner owner);
        void DeleteOwner(Owner owner);
    }
    ```
    ```
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationContext _context;

        public OwnerRepository(ApplicationContext context)
        {
            _context = context;
        }
        public IEnumerable<Owner> GetAll() => _context.Owners.ToList();
        public Owner GetById(Guid id) => _context.Owners.SingleOrDefault(o => o.Id.Equals(id));
        public Owner CreateOwner(Owner owner)
        {
            owner.Id = Guid.NewGuid();
            _context.Add(owner);
            _context.SaveChanges();
            return owner;
        }
        public Owner UpdateOwner(Owner dbOwner, Owner owner)
        {
            dbOwner.Name = owner.Name;
            dbOwner.Address = owner.Address;

            _context.SaveChanges();

            return dbOwner;
        }
        public void DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            _context.SaveChanges();
        }
    }
    ```
    ```
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(IOwnerRepository repository)
        {
            Field<OwnerType>(
                "createOwner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OwnerInputType>> { Name = "owner"}),
                resolve: context =>
                {
                    var owner = context.GetArgument<Owner>("owner");
                    return repository.CreateOwner(owner);
                }
            );
            Field<OwnerType>(
                "updateOwner",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<OwnerInputType>> { Name = "owner"},
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "ownerId" }),
                resolve: context => 
                {
                    var owner = context.GetArgument<Owner>("owner");
                    var ownerId = context.GetArgument<Guid>("ownerId");

                    var dbOwner = repository.GetById(ownerId);
                    if(dbOwner == null)
                    {
                        context.Errors.Add(new ExecutionError("Couldn't find owner in db"));
                        return null;
                    }

                    return repository.UpdateOwner(dbOwner, owner);
                }
            );
            Field<StringGraphType>(
                "deleteOwner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "ownerId" }),
                resolve: context => 
                {
                    var ownerId = context.GetArgument<Guid>("ownerId");
                    var owner = repository.GetById(ownerId);
                    if (owner == null){
                        context.Errors.Add(new ExecutionError("Could not find owner in"));
                        return null;
                    }

                    repository.DeleteOwner(owner);
                    return $"The owner with the id {ownerId} has been successfully deleted from db.";
                }
            );
        }
    }
    ```
46. Now lets run the project and check out ***delete*** action
    ```
    mutation($ownerId: ID!){
    deleteOwner(ownerId: $ownerId)
    }
    {
    "ownerId" : "c4ed9f1c-f112-4bd8-8d9f-3cfb7ee50e57"
    }
    ```
    Reference till this stage: 
    - https://code-maze.com/graphql-mutations/
    - https://github.com/CodeMazeBlog/graphql-series/tree/graphql-mutations

47. From here, we are going to make a dotnet web api project to consume the graphQl service. lets create a ***webApi*** project in the ***GraphQlPlaybook*** directory
    ```
    dotnet new webapi -n GraphQlClient
    ```
    ```
    dotnet sln add GraphQlClient/GraphQlClient.csproj
    ```
48. Go to the ***appsettings.json*** and add ***GraphQLURI***
    ```
    {
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
        }
    },
    "GraphQLURI" : "https://localhost:5001/graphql"
    }
    ```
49. add ***packages*** to the project
    ```
    dotnet add package GraphQL.Client
    dotnet add package GraphQL.Client.Serializer.Newtonsoft
    ```
50. Go to ***program.cs*** file and add ***GraphQLClient*** integration
    ```
    var graphQLURI = builder.Configuration["GraphQLURI"];
    if (string.IsNullOrWhiteSpace(graphQLURI))
    {
        throw new ArgumentNullException(nameof(graphQLURI), "GraphQL URI is not configured.");
    }
    services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(Configuration["GraphQLURI"], new NewtonsoftJsonSerializer()));
    ```
51. Now create a class ***OwnerConsumer***
    ```
    dotnet new class -n OwnerConsumer
    ```
    ```
    public class OwnerConsumer
    {
        private readonly IGraphQLClient _client;
        public OwnerConsumer(IGraphQLClient client)
        {
            _client = client;
        }
    }
    ```
52. Go to ***program.cs*** file and register ***OwnerConsumer***
    ```
    services.AddScoped<OwnerConsumer>();
    ```
53. Create a new folder ***Models*** and add the following classes(we need the same model classes in the client).
    ```
    mkdir Models
    cd Models
    ```
    ```
    dotnet new class -n TypeOfAccount
    dotnet new class -n Account
    dotnet new class -n Owner
    dotnet new class -n OwnerInput
    ```
    ```
    public enum TypeOfAccount
    {
        Cash,
        Savings,
        Expense,
        Income
    }
    public class Account
    {
        public Guid Id { get; set; }
        public TypeOfAccount Type { get; set; }
        public string Description { get; set; }
    }
    public class Owner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
    public class OwnerInput
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
    ```
54. Lets go to ***OwnerConsumer*** and update it with new methods
    ```
    public class OwnerConsumer
    {
        private readonly IGraphQLClient _client;
        public OwnerConsumer(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<List<Owner>> GetAllOwners()
        {
            var query = new GraphQLRequest
            {
                Query = @"query ownersQuery{
                            owners{
                                id
                                name
                                address
                                accounts {
                                    id
                                    type
                                    description
                                }
                            }
                        }"
            };

            var response = await _client.SendQueryAsync<ResponseOwnerCollectionType>(query);
            return response.Data.Owners;
        }
    }
    ```
55. Let's create a folder for the responseTypes and add response types
    ```
    mkdir ResponseTypes
    cd ResponseTypes
    dotnet new class -n ResponseOwnerCollectionType
    dotnet new class -n ResponseOwnerType
    ```
    ```
    public class ResponseOwnerCollectionType
    {
        public List<Owner> Owners { get; set; }
    }
    ```
    ```
    public class ResponseOwnerType
    {
        public Owner Owner { get; set; }
    }
    ```
56. Now lets add other query methods in the ***OwnerConsumer*** class
    ```
    public async Task<Owner> GetOwner(Guid id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query ownerQuery($ownerID: ID!) {
                  owner(ownerId: $ownerID) {
                    id
                    name
                    address
                    accounts {
                      id
                      type
                      description
                    }
                  }
                }",
            Variables = new { ownerID = id }
        };

        var response = await _client.SendQueryAsync<ResponseOwnerType>(query);
        return response.Data.Owner;
    }
    public async Task<Owner> CreateOwner(OwnerInput ownerToCreate)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                mutation($owner: ownerInput!){
                  createOwner(owner: $owner){
                    id,
                    name,
                    address
                  }
                }",
            Variables = new { owner = ownerToCreate }
        };

        var response = await _client.SendMutationAsync<ResponseOwnerType>(query);
        return response.Data.Owner;
    }

    public async Task<Owner> UpdateOwner(Guid id, OwnerInput ownerToUpdate)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                        mutation($owner: ownerInput!, $ownerId: ID!){
                        updateOwner(owner: $owner, ownerId: $ownerId){
                            id,
                            name,
                            address
                          }
                        }",
            Variables = new { owner = ownerToUpdate, ownerId = id }
        };

        var response = await _client.SendMutationAsync<ResponseOwnerType>(query);
        return response.Data.Owner;
    }

    public async Task<Owner> DeleteOwner(Guid id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
                        mutation($ownerId: ID!){
                          deleteOwner(ownerId: $ownerId)
                        }",
            Variables = new { ownerId = id }
        };

        var response = await _client.SendMutationAsync<ResponseOwnerType>(query);
        return response.Data.Owner;
    }
    ```
57. now lets add ***Controller*** and use ***OwnerConsumer*** to query
    ```
    mkdir Controllers
    dotnet new class -n OwnersController
    ```
    ```
    using Microsoft.AspNetCore.Mvc;

    namespace GraphQlClient;

    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly OwnerConsumer _consumer;
        public OwnersController(OwnerConsumer consumer)
        {
            _consumer = consumer;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var owners = await _consumer.GetAllOwners();
            return Ok(owners);
        }

        // GET api/values
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var owner = await _consumer.GetOwner(id);
            return Ok(owner);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OwnerInput owner)
        {
            var createdOwner = await _consumer.CreateOwner(owner);
            return Ok(createdOwner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] OwnerInput owner)
        {
            var updatedOwner = await _consumer.UpdateOwner(id, owner);
            return Ok(updatedOwner);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Put(Guid id)
        {
            var deleteOwner = await _consumer.DeleteOwner(id);
            return Ok(deleteOwner);
        }
    }
    ```
58. Now modify ***program.cs*** file to configure the ***controller***
    ```
    using GraphQL.Client.Abstractions;
    using GraphQL.Client.Http;
    using GraphQL.Client.Serializer.Newtonsoft;
    using GraphQlClient;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    var graphQLURI = builder.Configuration["GraphQLURI"];
    if (string.IsNullOrWhiteSpace(graphQLURI))
    {
        throw new ArgumentNullException(nameof(graphQLURI), "GraphQL URI is not configured.");
    }
    builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(graphQLURI, new NewtonsoftJsonSerializer()));

    builder.Services.AddScoped<OwnerConsumer>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
    ```
59. Now let's run ***GraphQlDotNetCore*** project from the cmd and check the port in which the application will be running. And then run the ***GraphQlClient*** project from the vsCode. But before running it, configure the ***GraphQLURI*** value with check running port in the  ***applicationSettings.json***. Now run and test and enjoy!


    Reference till this stage: 
    - https://code-maze.com/consume-graphql-api-with-asp-net-core/
    - https://github.com/CodeMazeBlog/graphql-series/tree/consuming-graphql-with-aspnetcore


## References
- https://code-maze.com/graphql-asp-net-core-tutorial/