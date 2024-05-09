using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Graph_1.Graph
{
    class Graph : IGraph
    {
        bool oriented;
        Dictionary<string, HashSet<string>> adjacencyList;

        public Graph()
        {
            adjacencyList = new Dictionary<string, HashSet<string>>();
        }

        public Graph(bool oriented) : this()
        {
            this.oriented = oriented;
        }

        public Graph(Dictionary<string, HashSet<string>> adjacencyList)
        {
            this.adjacencyList = adjacencyList;
        }

        public Graph(string file_in) : this()
        {
            using (StreamReader file = new StreamReader(file_in))
            {
                string[] graphType = file.ReadLine().Split(',');
                oriented = graphType[1] == "1";

                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    string[] vertices = ln.Split(',');
                    adjacencyList.Add(vertices[0], new HashSet<string>());
                    for (int i = 1; i < vertices.Length; i++)
                    {
                        if (vertices[i].Length > 0)
                        {
                            adjacencyList[vertices[0]].Add(vertices[i]);
                        }
                    }

                }
            }
        }

        public Graph(Graph otherGraph) : this()
        {
            foreach (var vertex in otherGraph.adjacencyList.Keys)
            {
                var otherAdjacentVertexList = otherGraph.adjacencyList[vertex];
                var adjacentVertexList = new HashSet<string>();
                adjacencyList.Add(vertex, adjacentVertexList);
                foreach (var adjacentVertex in otherAdjacentVertexList)
                {
                    adjacentVertexList.Add(adjacentVertex);
                }
            }
        }

        public void AddEdge(string vertex1, string vertex2)
        {
            if (!adjacencyList.ContainsKey(vertex1) && !adjacencyList.ContainsKey(vertex2))
            {
                throw new GraphException(string.Format("No vertex {0} and {1} in graph", vertex1, vertex2));
            }
            if (!adjacencyList.ContainsKey(vertex1))
            {
                throw new GraphException(string.Format("No vertex {0} in graph", vertex1));
            }
            if (!adjacencyList.ContainsKey(vertex2))
            {
                throw new GraphException(string.Format("No vertex {0} in graph", vertex2));
            }
            adjacencyList[vertex1].Add(vertex2);
            if (!oriented)
            {
                adjacencyList[vertex2].Add(vertex1);
            }
        }

        void IGraph.AddEdge(string vertex1, string vertex2, int weight)
        {
            throw new NotImplementedException();
        }

        void IGraph.RemoveEdge(string vertex1, string vertex2)
        {
            if (!adjacencyList.ContainsKey(vertex1) && !adjacencyList.ContainsKey(vertex2))
            {
                throw new GraphException(string.Format("No vertex {0} and {1} in graph", vertex1, vertex2));
            }
            if (!adjacencyList.ContainsKey(vertex1))
            {
                throw new GraphException(string.Format("No vertex {0} in graph", vertex1));
            }
            if (!adjacencyList.ContainsKey(vertex2))
            {
                throw new GraphException(string.Format("No vertex {0} in graph", vertex2));
            }
            adjacencyList[vertex1].Remove(vertex2);
            if (!oriented)
            {
                adjacencyList[vertex2].Remove(vertex1);
            }
        }

        public bool IsWeighted()
        {
            return false;
        }

        void IGraph.AddVertex(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList.Add(vertex, new HashSet<string>());
            }
            else
            {
                throw new GraphException(string.Format("Vertex {0} already exits in graph!", vertex));
            }
        }

        void IGraph.RemoveVertex(string vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
            {
                adjacencyList.Remove(vertex);
            }
            else
            {
                throw new GraphException(string.Format("Vertex {0} does not exit in graph!", vertex));
            }
        }

        void IGraph.Save(string file_out)
        {
            using (StreamWriter sw = new StreamWriter(file_out))
            {
                sw.Write(IsWeighted() ? "1" : "0");
                sw.Write(',');
                sw.Write(oriented ? "1" : "0");
                sw.WriteLine();
                foreach (var vertex in adjacencyList.Keys)
                {
                    sw.Write(vertex);
                    var adjacentVertexList = adjacencyList[vertex];
                    if (adjacentVertexList.Count != 0)
                    {
                        foreach (var adjacentVertex in adjacentVertexList)
                        {
                            sw.Write(',');
                            sw.Write(adjacentVertex);
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        string IGraph.ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in adjacencyList.Keys)
            {
                sb.Append(key).Append(":");

                var vertexList = adjacencyList[key];
                string str = string.Join(',', vertexList.ToArray());
                sb.Append(str);

                sb.Append("\n");
            }

            return sb.ToString();
        }

        //int IGraph.Degree(string vertex)
        //{
        //    return adjacencyList[vertex].Count;
        //}

        private int DegreeOut(string vertex)
        {
            return adjacencyList[vertex].Count;
        }

        List<string> IGraph.GetPendantVertices()
        {
            List<string> pendantVerticesList = new List<string>();
            if (!oriented)
            {
                foreach (string vertex in adjacencyList.Keys)
                {
                    if (DegreeOut(vertex) == 1)
                    {
                        pendantVerticesList.Add(vertex);
                    }
                }
            }
            
            return pendantVerticesList;
        }

        public bool IsDirected()
        {
            return oriented;
        }

        public List<string> GetAdjacentVertices(string vertex)
        {
            if (oriented)
            {   
                if (adjacencyList.ContainsKey(vertex))
                {
                    return adjacencyList[vertex].ToList();
                }
                else
                {
                    return new List<string>();
                }
                
            }
            else
            {
                throw new Exception(string.Format("Graph is undirected!"));
            }
        }

        public List<string> GetVertices()
        {
            return adjacencyList.Keys.ToList();
        }

        // метод получающий графы из списка смежных вершин с вершиной передаваемой в параметре vertex
        public IGraph GetAdjacencyComponent(string vertex)
        {
            
            HashSet<string> linkedVertices = new HashSet<string>();
            List<string> adjacentVertices = adjacencyList[vertex].ToList();
            linkedVertices.Add(vertex);

            foreach (string adjacentVertex in adjacentVertices)
            {
                AddAdjacentComponents(adjacentVertex, ref linkedVertices); // выззов рекурсивного метода получения списка связанных вершин
            }

            Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
            foreach (var linkedComponent in linkedVertices)
            {
                dictionary.Add(linkedComponent, new HashSet<string>(adjacencyList[linkedComponent].ToList()));
            }
            
            return new Graph(dictionary);
        }

        // рекурсивный метод получающий список вершин, входящих в компоненту связности графа
        private void AddAdjacentComponents(string vertex, ref HashSet<string> linkedComponents)
        {
            List<string> adjacentVertices = adjacencyList[vertex].ToList();
            linkedComponents.Add(vertex);
            
            foreach(string adjacentVertex in adjacentVertices)
            {
                if (!linkedComponents.Contains(adjacentVertex))
                {
                    AddAdjacentComponents(adjacentVertex, ref linkedComponents);
                }
            }
           
        }

        public List<IGraph> GetAdjacencyComponents()        // метод собирающий список компонент связности 
        {
            List<IGraph> components = new List<IGraph>();
            foreach(var vertex in GetVertices()) 
            {
                if (!VertexIsInLinkedComponents(vertex, components))
                {
                    IGraph graph = GetAdjacencyComponent(vertex);
                    components.Add(graph);
                }
            }

            return components;
        }

        private bool VertexIsInLinkedComponents(string vertex, List<IGraph> components) // метод определяющий содержится ли вершина в других компонентах связност
        {
            foreach (var graph in components) 
            {
                if (graph.GetVertices().Contains(vertex))
                {
                    return true;
                }
            }
            return false;
        }

        private void VisitVertices(string vertex, ref bool[] isVisited)
        {
             
        }

        public Dictionary<string, List<string>> GetShortestPaths(string vertex)
        {
            Dictionary<string, List<string>> allPaths = new Dictionary<string, List<string>>();
            HashSet<string> visitedVertices = new HashSet<string>();
            Queue<string> queue = new Queue<string>();

            queue.Enqueue(vertex);
            visitedVertices.Add(vertex);

            List<string> path = new List<string>();
            path.Add(vertex);
            allPaths.Add(vertex, path);

            while(queue.Count > 0) 
            {
                string currentVertex = queue.Dequeue();
                HashSet<string> adjacentVertices = adjacencyList[currentVertex];
                foreach (string adjacentVertex in adjacentVertices)
                {
                    if (!visitedVertices.Contains(adjacentVertex)) 
                    {
                        AddPathElement(allPaths, currentVertex, adjacentVertex);
                        visitedVertices.Add(adjacentVertex);
                        queue.Enqueue(adjacentVertex);
                    }
                }
            }

            //PrintAllPaths(allPaths);
            
            return allPaths;
        }

        private void PrintAllPaths(Dictionary<string, List<string>> allPaths)
        {
            foreach(string vertex in allPaths.Keys)
            {
                foreach(string node in allPaths[vertex])
                {
                    Console.Write(node);
                    Console.Write(" ");
                }
                Console.WriteLine();
                
            }

        }

        private void AddPathElement(Dictionary<string, List<string>> allPaths, string currentVertex, string adjacentVertex)
        {
            List<string> currentPath = allPaths[currentVertex];

            List<string> adjacentPath = new List<string>(currentPath);
            adjacentPath.Add(adjacentVertex);
                       
            allPaths.Add(adjacentVertex, adjacentPath);    

        }

        public Tuple<IGraph, int> Kruskal()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> Dijkstra(string vertex)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> BellmanFord(string vertex)
        {
            throw new NotImplementedException();
        }

        public int[,] Floyd()
        {
            throw new NotImplementedException();
        }

        public bool HasNegativeCycle(string vertex)
        {
            throw new NotImplementedException();
        }

        public int FordFulkerson(string src, string dest)
        {
            throw new NotImplementedException();
        }

        public List<string> Bfs(Dictionary<string, Dictionary<string, int>> adjacencyList, string src, string dest)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Dictionary<string, int>> GetAdjacencyList()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> HasNegativeCycle(string vertex, ref HashSet<string> negativeVertices)
        {
            throw new NotImplementedException();
        }
    }
}
