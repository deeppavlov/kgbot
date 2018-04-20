using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    public abstract class MessageProcessingMiddleware : IMiddleware
    {
        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                await OnMessage(context); 
            }
            await next();
        }

        public abstract Task OnMessage(ITurnContext context);
    }
}
