using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    /// <summary>
    /// A class to save a queue of utterances of a given size
    /// </summary>
    public class UtteranceQueueMiddleware<T> : MessageProcessingMiddleware where T: TState,new()
    {
        public int N { get; set; } = 5;

        public UtteranceQueueMiddleware(int N)
        {
            this.N = N;
        }

        public static string[] GetUtteranceHistory(T State)
        {
            var q = (Queue<string>)State.SysMem["UQueue"];
            return q.Count > 0 ? q.ToArray() : null;
        }

        public static void Reset(T State)
        {
            var q = (Queue<string>)State.SysMem["UQueue"];
            q.Clear();
        }

        public async override Task OnMessage(ITurnContext context)
        {
            var SysMem = UserState<T>.Get(context).SysMem;
            if (!SysMem.ContainsKey("UQueue")) SysMem.Add("UQueue",new Queue<string>());
            var UQueue = (Queue<string>)SysMem["UQueue"];
            UQueue.Enqueue(context.Activity.Text);
            if (UQueue.Count >= N) UQueue.Dequeue();
        }
    }
}
