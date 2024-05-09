using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_1.Graph
{
    [Serializable]
    class GraphException : Exception
    {
        public GraphException() { }

        public GraphException(string message)
            : base(message)
        {

        }
    }
}
