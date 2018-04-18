using KudaBot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaBot.KGBot
{
    public class KGBState : TState
    {
        public Dictionary<string, string> AV { get; set; } = new Dictionary<string, string>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            foreach(var x in AV.Keys)
            {
                sb.Append($" - AV[{x}]={AV[x]}\n");
            }
            return sb.ToString();
        }

    }
}
