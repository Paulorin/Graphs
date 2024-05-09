using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_1.Graph
{
    public interface IGraph
    {
        void AddVertex(string vertex);
        void RemoveVertex(string vertex);
        void Save(string file_out);
        public void AddEdge(string vertex1, string vertex2, int weight);

        public void AddEdge(string vertex1, string vertex2);
        void RemoveEdge(string vertex1, string vertex2);
        public string ToString();
        public bool IsWeighted();
        public bool IsDirected();
        public List<string> GetAdjacentVertices(string vertex);
        public List<string> GetVertices();
        public List<IGraph> GetAdjacencyComponents();
        public List<string> GetPendantVertices();
        Dictionary<string, List<string>> GetShortestPaths(string vertex);
        public Tuple<IGraph, int> Kruskal();
        Dictionary<string, int> Dijkstra(string vertex);
        Dictionary<string, int> BellmanFord(string vertex);
        int[,] Floyd();
        Dictionary<string, int> HasNegativeCycle(string vertex, ref HashSet<string> negativeVertices);

        List<string> Bfs(Dictionary<string, Dictionary<string, int>> adjacencyList, string src, string dest);
        int FordFulkerson(string src, string dest);
        Dictionary<string, Dictionary<string, int>> GetAdjacencyList();

        
    }
}
