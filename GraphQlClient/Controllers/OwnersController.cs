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
