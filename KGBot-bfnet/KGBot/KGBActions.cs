using KudaBot.Middleware;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.KGBot
{
    public class KGBActions : IActionable
    {
        [Keyword("help")]
        [Keyword("помоги")]
        public async Task Help(ITurnContext ctx)
        {
            await ctx.SendActivity("Hello! I am KudaGo Bot! You can ask me for *places* to go!");
        }


        [Keyword("places")]
        [Keyword("place")]
        public async Task Places(ITurnContext ctx)
        {
            var K = new KudaGo.KudaGo();
            await ctx.SendActivity(MessageFactory.Carousel(await K.Places()));
        }

        [Keyword("/state")]
        public async Task PrintState(ITurnContext ctx)
        {
            await ctx.SendActivity($"{UserState<KGBState>.Get(ctx)}");
        }

        [Keyword("/reset")]
        public async Task ResetState(ITurnContext ctx)
        {
            var S = ctx.GetUserState<KGBState>();
            S.PavlovState = new PavlovState();
            UtteranceQueueMiddleware<KGBState>.Reset(S);
            await ctx.SendActivity($"Ок, начинаем заново");
        }


        [Entity("date-time")]
        public async Task DateTime(ITurnContext ctx, string value)
        {
            // UserState<KGBState>.Get(ctx).AV.Add("date-time", value);
        }

    }
}
