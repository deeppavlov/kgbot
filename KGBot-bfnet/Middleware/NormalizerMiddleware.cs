using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    public class NormalizerMiddleware : IMiddleware
    {
        public bool Lowcase { get; set; }

        public NormalizerMiddleware(bool Lowcase)
        {
            this.Lowcase = Lowcase;
        }

        public NormalizerMiddleware() : this(false) { }

        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            context.Activity.Text = context.Activity.Text == null ? string.Empty : context.Activity.Text;
            if (Lowcase) context.Activity.Text = context.Activity.Text.ToLower().Trim();
            await next();
        }
    }
}
