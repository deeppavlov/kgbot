using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KudaBot.Middleware
{
    /// <summary>
    /// This is the base class for all custom state objects. 
    /// It provides system dictionary used by other middleware classes.
    /// </summary>
    public class TState
    {
        public TState() { }
        public Dictionary<string, object> SysMem { get; set; } = new Dictionary<string, object>();
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var x in SysMem.Keys)
            {
                sb.AppendLine($" - SysMem[{x}]={SysMem[x]}");
            }
            return sb.ToString();
        }
    }
}
