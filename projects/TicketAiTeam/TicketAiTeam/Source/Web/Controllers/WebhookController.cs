namespace TicketAiTeam.Web.Controllers;

using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using TicketAiTeam.Web.Model;

[Controller]
[Route("youtrack/webhook")]
public sealed class WebhookController : ControllerBase
{
  private readonly ChannelWriter<WebhookEvent> _channel;

  public WebhookController(ChannelWriter<WebhookEvent> channel)
  {
    _channel = channel;
  }

  [HttpGet]
  public IActionResult Get()
  {
    return Ok("Something is happening!");
  }
  
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] WebhookEvent @event)
  {
    await _channel.WriteAsync(@event, HttpContext.RequestAborted);

    return NoContent();
  }
}