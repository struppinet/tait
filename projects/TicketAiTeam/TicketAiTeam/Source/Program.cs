using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TicketAiTeam.Configuration;
using TicketAiTeam.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOptions<ApplicationOptions>().Bind(builder.Configuration.GetSection(ApplicationOptions.SectionName));
builder.Services.AddTaitMcpServer();
builder.Services.AddYouTrackClient();
builder.Services.AddWebhookHandler();
builder.Services.AddChatClient();

var app = builder.Build();

app.LogEnvironmentInformationOnStartup();
app.MapMcp();
app.MapControllers();

app.Run();