using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true)]
    public class EntityAttribute : Attribute
    {
        public string EntityName { get; set; }
        public EntityAttribute(string Name)
        {
            this.EntityName = Name;
        }
    }

    public class DialogFlowMiddleware : IMiddleware
    { 
        public IActionable ActionClass { get; set; }

        public string ApiKey { get; set; }

        public bool Fired { get; set; }

        public DialogFlowMiddleware(string ApiKey, IActionable ActionClass)
        {
            this.ActionClass = ActionClass;
            this.ApiKey = ApiKey;
        }

        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            Fired = false;

            var q = System.Web.HttpUtility.UrlEncode(context.Activity.Text);

            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", "Bearer " + ApiKey);
            string res;
            res = await http.GetStringAsync("https://api.dialogflow.com/v1/query?sessionId=1&lang=ru&v=20150910&query=" + q);
            dynamic jres = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            if (ActionClass != null)
            {
                foreach (var x in ActionClass.GetType().GetMethods())
                {
                    var A = x.GetCustomAttributes(typeof(EntityAttribute), false);
                    if (A != null)
                    {
                        foreach (var t in A)
                        {
                            var val = jres.result.parameters[((EntityAttribute)t).EntityName];
                            if (val != null)
                            {
                                await (Task)x.Invoke(ActionClass, new object[] { context,val });
                            }
                        }
                    }
                }
            }
            await next();
        }
    }
}
