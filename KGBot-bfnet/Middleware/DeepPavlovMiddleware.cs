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
        private string ApiUri { get; set; }

        public DeepPavlovMiddleware(string apiUri)
        {
            this.ApiUri = apiUri;
        }

        private static CardImage[] GetImages(dynamic l, int n = 1)
        {
            List<CardImage> res = new List<CardImage>();
            foreach (var x in l)
            {
                res.Add(new CardImage(x.ToString()));
                if (--n == 0) break;
            }
            return res.ToArray();
        }

        private List<Attachment> BuildEventsAttachment(dynamic list)
        {
            var attachments = new List<Attachment>();
            foreach (var x in list)
            {
                attachments.Add(new HeroCard()
                {
                    Text = "[" + x.title.ToString() + "]("  + x.url.ToString() + ")",
                    Images = GetImages(x.images)
                }.ToAttachment());
            }
            return attachments;
        }

        public override async Task OnMessage(ITurnContext context)
        {
            var S = UserState<KGBState>.Get(context);
            var R = new PavlovRequest(S.PavlovState);
            R.utter_history = UtteranceQueueMiddleware<KGBState>.GetUtteranceHistory(S);
            R.utterance = context.Activity.Text;
            var shown_events = S.PavlovState.slot_history.ContainsKey("shown_events") ? (List<int>)S.PavlovState.slot_history["shown_events"] : new List<int>();
#if PRINT_STATE
            await context.SendActivity(JsonConvert.SerializeObject(S.PavlovState));
#endif
            var http = new HttpClient();
            var s = System.Web.HttpUtility.JavaScriptStringEncode(context.Activity.Text);
            var c = new StringContent(JsonConvert.SerializeObject(R),Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(ApiUri, c);
            var res = await resp.Content.ReadAsStringAsync();
#if PRINT_STATE
            await context.SendActivity(res);
#endif
            dynamic jres = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            S.PavlovState.last_cluster_id = jres[3];
            var shs = jres[2].ToString();
            S.PavlovState.slot_history = JsonConvert.DeserializeObject<Dictionary<string, object>>(shs);
            foreach (var t in jres[1])
            {
                var lid = (int)t.local_id;
                if (!shown_events.Contains(lid)) shown_events.Add(lid);
            }
            if (S.PavlovState.slot_history.ContainsKey("shown_events"))
            {
                S.PavlovState.slot_history["shown_events"] = shown_events;
            }
            else
            {
                S.PavlovState.slot_history.Add("shown_events", shown_events);
            }
            if (jres[0] is JValue) // this is single-line response
            {
                var msg = MessageFactory.Carousel(BuildEventsAttachment(jres[1]));
                msg.Text = jres[0].ToString();
                await context.SendActivity(msg);
                // await context.SendActivity($"{(string)jres[0]}\r\n{BuildEvents(jres[1])}");
            }
            else if (jres[0] is JArray)
            {
                // await context.SendActivity($"Вот что мы вам рекомендуем:\r\n{BuildEvents(jres[0])}");
            }
            else
            { }
            
        }
    }
}
