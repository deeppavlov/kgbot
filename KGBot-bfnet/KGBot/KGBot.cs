using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.KGBot
{
    public class KGBot : IBot
    {
        public async Task OnTurn(ITurnContext ctx)
        {
            if (ctx.Activity.Type is ActivityTypes.Message)
            {
                if (!ctx.Responded)
                {
                    await ctx.SendActivity("Hello! I am KudaGo bot! You can say <<help>> to see what you can ask");
                }
            }
        }
    }
}
