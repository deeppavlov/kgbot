using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KudaBot.KudaGo
{
    public class KudaGo
    {

        public static async Task<dynamic> ApiCall(string url)
        {
            var http = new HttpClient();
            var res = await http.GetStringAsync(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(res);
        }

        public static CardImage[] GetImages(dynamic l, int n=0)
        {
            List<CardImage> res = new List<CardImage>();
            foreach (var x in l)
            {
                res.Add(new CardImage(x.image.ToString()));
                if (n-- == 0) break;
            }
            return res.ToArray();
         }

        public async Task<Attachment[]> Places()
        {
            var L = new List<Attachment>();
            var p = await ApiCall("https://kudago.com/public-api/v1.2/places?fields=title,images");
            foreach(var x in p.results)
            {
                L.Add(new HeroCard()
                {
                    Text = x["title"].ToString(),
                    Images = GetImages(x.images)
                }.ToAttachment());
            }
            return L.ToArray();
        }
    }
}
