using KudaBot.KGBot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    public interface IActionable { };

    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true)]
    public class KeywordAttribute : Attribute
    {
        public string Text { get; set; }
        public KeywordAttribute(string Text)
        {
            this.Text = Text;
        }
    }

    public class KeywordRecognizerMiddleware : IMiddleware
    { 
        public IActionable ActionClass { get; set; }

        public bool Fired { get; set; }

        public KeywordRecognizerMiddleware(IActionable ActionClass)
        {
            this.ActionClass = ActionClass;
        }

        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            Fired = false;
            var w = context.Activity.Text.Split(' ', ',', '-', '.', '!', '(', ')').Select(x => x.Trim()).Where(x => x.Length > 0);

            foreach (var x in ActionClass.GetType().GetMethods())
            {
                var A = x.GetCustomAttributes(typeof(KeywordAttribute),false);
                if (A!=null)
                {
                    foreach(var t in A)
                    {
                        if (w.Contains(((KeywordAttribute)t).Text))
                        {
                            UserState<KGBState>.Get(context).AV.Add(((KeywordAttribute)t).Text, "1");
                            await (Task) x.Invoke(ActionClass, new object[] { context });
                            Fired = true;
                        }
                    }
                }
            }
            await next();
        }
    }
}
