using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    public class DeepPavlovMiddleware : IMiddleware
    {
        public string ApiUri { get; set; }

        public DeepPavlovMiddleware(string ApiUri)
        {
            this.ApiUri = ApiUri;
        }

        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            var http = new HttpClient();
            var s = System.Web.HttpUtility.JavaScriptStringEncode(context.Activity.Text);
            var c = new StringContent($"{{\"context\":\"{s}\"}}",Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(ApiUri, c);
            var res = await resp.Content.ReadAsStringAsync();
            dynamic jres = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            var fst = true;
            var sb = new StringBuilder();
            foreach (var x in jres)
            {
                if (fst)
                {
                    sb.AppendLine("Мы нашли следующие события:"); fst = false;
                }
                sb.AppendFormat($" * [{x.title}]({x.url})");
            }
            if (!fst) await context.SendActivity(sb.ToString());
            await next();
        }
    }
}
