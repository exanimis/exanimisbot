using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ExanimisBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
       

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public DiscordSocketClient Client { get => _client; set => _client = value; }

        public async Task RunBotAsync()
        {
            Client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = "MzcwMTQ3NjgwNzIwMTI1OTU0.DTImbg.c5xwHKfhtFmkoubgNBukfCqsdcE";

            //event subscriptions
            Client.Log += Log;


            await RegisterCommandsAsync();

            await Client.LoginAsync(TokenType.Bot, botToken);
            await Client.StartAsync();

            await Task.Delay(-1);

        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }


        public async Task RegisterCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync; 
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos =0;

            if (message.HasStringPrefix("/", ref argPos) || message.HasMentionPrefix(Client.CurrentUser,ref argPos))
            {
                var context = new SocketCommandContext(Client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);

            }
        }
    }
}
