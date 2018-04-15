using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;

namespace CSSBot
{
    public class Bot
    {
        private DiscordSocketClient m_client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task Start()
        {
            // starts our client
            // we use LogSeverity.Debug because more info the better
            m_client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = LogSeverity.Debug });

            // init the Command Service
            _commands = new CommandService();

            // log in as a bot using our connection token
            await m_client.LoginAsync(TokenType.Bot, Program.GlobalConfiguration.Data.ConnectionToken);
            await m_client.StartAsync();

            // dependency injection
            // this is used to automatically populate the types that commands ask
            // for, since we don't instantiate the types ourselves
            _services = new ServiceCollection()
                .AddSingleton(m_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            
            await InstallCommandsAsync();

            // set up our logging function
            m_client.Log += Log;

            // show an invite link when we are ready to go
            m_client.Ready += Client_Ready;

            // set some help text
            // this is a good way to let the user know which command to type to get started
            await m_client.SetGameAsync(string.Format("Type {0}Help", GlobalConfiguration.CommandPrefix));

            // wait indefinitely 
            await Task.Delay(-1);
        }

        private async Task InstallCommandsAsync()
        {
            m_client.MessageReceived += client_MessageReceived;
            await _services.GetRequiredService<CommandService>().AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task client_MessageReceived(SocketMessage arg)
        {
            // Don't handle the command if it is a system message
            var message = arg as SocketUserMessage;
            if (message == null) return;

            // Mark where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message has a valid prefix, adjust argPos 

            // ensure that the message either starts with the command prefix
            // or by @mentioning the bot user
            if (!(message.HasMentionPrefix(m_client.CurrentUser, ref argPos) || message.HasCharPrefix(GlobalConfiguration.CommandPrefix, ref argPos))) return;

            // Create a Command Context
            var context = new CommandContext(m_client, message);
            // Execute the Command, store the result
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            // If the command failed
            if (!result.IsSuccess)
            {
                // log the error
                LogMessage errorMessage = new LogMessage(LogSeverity.Warning, "CommandHandler", result.ErrorReason);
                await Log(errorMessage);
            }
        }

        private async Task Client_Ready()
        {
            // display a helpful invite url in the log when the bot is ready
            var application = await m_client.GetApplicationInfoAsync();
            await Log(new LogMessage(LogSeverity.Info, "Program",
                $"Invite URL: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>"));
        }

        public async static Task Log(Discord.LogMessage arg)
        { 
            // log stuff to console
            // could also log to a file here
            Console.WriteLine(arg.ToString());
        }
    }
}
