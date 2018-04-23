using KudaBot.Middleware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaBot.KGBot
{
    public class PavlovState
    {
        public Dictionary<string, object> slot_history { get; set; } = new Dictionary<string, object>();
        public string last_cluster_id { get; set; } = null;
    }

    public class PavlovRequest : PavlovState
    {
        public string utterance { get; set; }
        public string[] utter_history { get; set; }

        public PavlovRequest() { }
        public PavlovRequest(PavlovState S)
        {
            this.last_cluster_id = S.last_cluster_id;
            this.slot_history = S.slot_history;
        }

    }

    
    public class KGBState : TState
    {
        public PavlovState PavlovState { get; set; } = new PavlovState();
        public override string ToString()
        {
            return $"PavlovState: {JsonConvert.SerializeObject(PavlovState)}, SysMem: {JsonConvert.SerializeObject(SysMem)}";
        }

    }
}
