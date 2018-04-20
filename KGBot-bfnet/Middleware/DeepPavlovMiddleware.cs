using KudaBot.KGBot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    public class DeepPavlovMiddleware : MessageProcessingMiddleware
    {
        public string ApiUri { get; set; }

        public DeepPavlovMiddleware(string ApiUri)
        {
            this.ApiUri = ApiUri;
        }

        public async override Task OnMessage(ITurnContext context)
        {
            var S = UserState<KGBState>.Get(context);
            var R = new PavlovRequest(S.PavlovState);
            R.utter_history = UtteranceQueueMiddleware<KGBState>.GetUtteranceHistory(S);
            R.utterance = context.Activity.Text;
            await context.SendActivity(JsonConvert.SerializeObject(S.PavlovState));
            var http = new HttpClient();
            var s = System.Web.HttpUtility.JavaScriptStringEncode(context.Activity.Text);
            var c = new StringContent(JsonConvert.SerializeObject(R),Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(ApiUri, c);
            var res = await resp.Content.ReadAsStringAsync();
            await context.SendActivity(res);
            dynamic jres = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            S.PavlovState.last_cluster_id = jres[2];
            S.PavlovState.slot_history = JsonConvert.DeserializeObject<Dictionary<string, string>>(jres[1].ToString());
            if (jres[0] is JValue) // this is single-line response
            {
                await context.SendActivity((string)jres[0]);
            }
            else if (jres[0] is JArray)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Мы рекомендуем:");
                foreach (var x in jres[0])
                {
                    sb.AppendLine($" * [{x.title}]({x.url})");
                }
                await context.SendActivity(sb.ToString());
            }
            else
            { }
            
        }
    }
}
