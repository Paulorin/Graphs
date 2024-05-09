using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Graph_1.Graph
{   
    class Edge : IComparable<Edge>
    {
        public string src, dest;
        public int weight;

        public Edge(string src, string dest, int weight)
        {
            this.src = src;
            this.dest = dest;
            this.weight = weight;
        }
        
        public Edge(Edge edge)
        {
            src = edge.src;
            dest = edge.dest;
            weight = edge.weight;
        }

        public Edge()
        {

        }

        public int CompareTo(Edge compareEdge)
        {
            return weight - compareEdge.weight;
        }
    }

    class NodeDistance : IComparable<NodeDistance>
    {
        public string vertex;
        public int distance;
        
        public NodeDistance()
        {

        }

        public NodeDistance(string vertex, int distance) 
        {
            this.vertex = vertex;
            this.distance = distance;
        }

        public int CompareTo(NodeDistance? otherNode)
        {
            return distance - otherNode.distance;
        }
    }

    class Subset
    {
        public string parent;
        public int rank;

        public Subset(string parent, int rank) 
        {
            this.parent = parent;
            this.rank = rank;
        }
    }

    class WeightedGraph : IGraph
    {
        bool oriented;
        public Dictionary<string, Dictionary<string, int>> adjacencyList;

        public WeightedGraph()
        {
            adjacencyList = new Dictionary<string, Dictionary<string, int>>();
        }

        public WeightedGraph(bool oriented) : this()
        {
            this.oriented = oriented;
        }

        public WeightedGraph(Dictionary<string, Dictionary<string, int>> adjacencyList)
        {
            this.adjacencyList = adjacencyList;
        }

        public WeightedGraph(string file_in) : this()
        {
            using (StreamReader file = new StreamReader(file_in))
            {
                string[] graphType = file.ReadLine().Split(',');
                oriented = graphType[1] == "1";
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    string[] vertices = ln.Split(',');
                    adjacencyList.Add(vertices[0], new Dictionary<string, int>());
                    for (int i = 1; i < vertices.Length - 1; i += 2)
                    {
                        adjacencyList[vertices[0]].Add(vertices[i], int.Parse(vertices[i + 1]));
                    }

                }
            }
        }

        public WeightedGraph(WeightedGraph otherGraph) : this()
        {
            foreach (var key in otherGraph.adjacencyList.Keys)
            {
                var otherVertexList = otherGraph.adjacencyList[key];
                var vertexList = new Dictionary<string, int>();
                adjacencyList.Add(key, vertexList);
                foreach (var vertex in otherVertexList.Keys)
                {
                    vertexList.Add(vertex, otherVertexList[vertex]);
                }
            }
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in adjacencyList.Keys)
            {
                sb.Append(key).Append(":");

                var vertexList = adjacencyList[key];
                foreach (var vertex in vertexList.Keys)
                {
                    sb.Append(vertex).Append(",").Append(vertexList[vertex]).Append(" ");
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public void AddVertex(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList.Add(vertex, new Dictionary<string, int>());
            }
            else
            {
                throw new GraphException(string.Format("Vertex {0} already exits in graph!", vertex));
            }
            
        }

        public void RemoveVertex(string vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
            {
                adjacencyList.Remove(vertex);
                foreach (var key in adjacencyList.Keys)
                {
                    var vertexList = adjacencyList[key];
                    vertexList.Remove(vertex);
                }
            }
            else
            {
                throw new GraphException(string.Format("Vertex {0} does not exit in graph!", vertex));
            }
        }

        public void Save(string file_out)
        {
            using (StreamWriter sw = new StreamWriter(file_out))
            {
                sw.Write(IsWeighted() ? "1" : "0");
                sw.Write(',');
                sw.Write(oriented ? "1" : "0");
                sw.WriteLine();
                foreach (var key in adjacencyList.Keys)
                {
                    sw.Write(key);
                    var vertexList = adjacencyList[key];
                    if (vertexList.Count != 0)
                    {
                        foreach (var vertex in vertexList.Keys)
                        {
                            sw.Write(',');
                            sw.Write(vertex);
                            sw.Write(',');
                            sw.Write(vertexList[vertex]);
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        public void AddEdge(string vertex1, string vertex2, int weight)
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
            adjacencyList[vertex1].Add(vertex2, weight);
            if (!oriented)
            {
                adjacencyList[vertex2].Add(vertex1, weight);
            }
        }

        public void RemoveEdge(string vertex1, string vertex2)
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

        public void AddEdge(string vertex1, string vertex2)
        {
            throw new NotImplementedException();
        }

        public bool IsWeighted()
        {
            return true;
        }

        public int OutcomingDegree(string vertex)
        {
            return adjacencyList[vertex].Count;
        }

        public List<string> GetPendantVertices()
        {
            List<string> pendantVeticesList = new List<string>();
            if (!oriented)
            {
                foreach (var vertex in adjacencyList.Keys)
                {
                    if (Degree(vertex) == 1)
                    {
                        pendantVeticesList.Add(vertex);
                    }
                }
            }

            return pendantVeticesList;
        }

        public int Degree(string vertex)
        {
            return adjacencyList[vertex].Count;
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
                    return adjacencyList[vertex].Keys.ToList();
                }
                else
                {
                    throw new GraphException(string.Format("No vertex {0} in graph", vertex));
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

        public IGraph GetAdjacencyComponent(string vertex)
        {
            throw new NotImplementedException();
        }

        public List<IGraph> GetAdjacencyComponents()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<string>> GetShortestPaths(string vertex)
        {
            throw new NotImplementedException();
        }

        public Tuple<IGraph, int> Kruskal()
        {
            List<Edge> edgeList = new List<Edge>();
            foreach(string vertex in adjacencyList.Keys)
            {
                Dictionary<string, int> adjacentVertices = adjacencyList[vertex];
                foreach(string adjacentVertex in adjacentVertices.Keys)
                {
                    edgeList.Add(new Edge(vertex, adjacentVertex, adjacentVertices[adjacentVertex]));
                }
            }

            edgeList.Sort();

            Edge[] mstEdges = new Edge[adjacencyList.Count - 1];
            int i = 0;
            for (i = 0; i < mstEdges.Length; i++)
            {
                mstEdges[i] = new Edge();
            }

            Dictionary<string, Subset> subsets = new Dictionary<string, Subset>();
            foreach (string vertex in adjacencyList.Keys)
            {
                subsets.Add(vertex, new Subset(vertex, 0));
            }

            i = 0;
            int edgeCount = 0;
            while(edgeCount < adjacencyList.Count - 1 && i < edgeList.Count)
            {
                // Pick the smallest edge. And increment the index for next iteration 
                Edge next_edge = edgeList[i++];

                string x = find(subsets, next_edge.src);
                string y = find(subsets, next_edge.dest);

                // If including this edge doesn't cause cycle, 
                // include it in result and increment the index 
                // of result for next edge 
                if (x != y)
                {
                    mstEdges[edgeCount++] = next_edge;
                    Union(subsets, x, y);
                }
            }

            WeightedGraph graph = new WeightedGraph();
            foreach (string vertex in adjacencyList.Keys)
            {
                if (adjacencyList[vertex].Count > 0)
                {
                    graph.AddVertex(vertex);
                } 
            }

            int commonWeight = 0;
            foreach(Edge edge in mstEdges) 
            {
                if (edge.src != null && edge.dest != null)
                {
                    graph.AddEdge(edge.src, edge.dest, edge.weight);
                    commonWeight += edge.weight;
                }
            }


            Console.WriteLine("MST edges:");
            foreach (Edge edge in mstEdges)
            {
                Console.WriteLine("{0},{1},{2}", edge.src, edge.dest, edge.weight);
            }
            Console.WriteLine();


            return new Tuple<IGraph, int>(graph, commonWeight);
        }

        private string find(Dictionary<string, Subset> subsets, string src)
        {
            // Find root and make root as parent of i (path compression) 
            if (subsets[src].parent != src)
                subsets[src].parent = find(subsets, subsets[src].parent);

            return subsets[src].parent;
        }

        private void Union(Dictionary<string, Subset> subsets, string x, string y)
        {
            string xroot = find(subsets, x);
            string yroot = find(subsets, y);

            // Attach smaller rank tree under root of high rank tree (Union by Rank) 
            if (subsets[xroot].rank < subsets[yroot].rank)
                subsets[xroot].parent = yroot;
            else if (subsets[xroot].rank > subsets[yroot].rank)
                subsets[yroot].parent = xroot;

            // If ranks are same, then make one as root and increment its rank by one 
            else
            {
                subsets[yroot].parent = xroot;
                subsets[xroot].rank++;
            }
        }

        public Dictionary<string, int> Dijkstra(string vertex)
        {
            Dictionary<string, int> distances = new Dictionary<string, int>();

            foreach(string node in adjacencyList.Keys)
            {
                distances.Add(node, int.MaxValue);
            }

            distances[vertex] = 0;

            SortedSet<NodeDistance> pq = new SortedSet<NodeDistance>();
            pq.Add(new NodeDistance(vertex, 0));

            while(pq.Count > 0)
            {
                NodeDistance minNodeDist = pq.Min;
                pq.Remove(minNodeDist);
                string u = minNodeDist.vertex;

                Dictionary<string, int> adjacentVertices = adjacencyList[u];
                foreach (string v in adjacentVertices.Keys)
                {
                    int weight = adjacentVertices[v];

                    if (distances[v] > distances[u] + weight)
                    {
                        distances[v] = distances[u] + weight;
                        pq.Add(new NodeDistance(v, distances[v]));
                    } 
                }
            }

            return distances;
        }

        public Dictionary<string, int> BellmanFord(string src)
        {
            List<Edge> edgeList = new List<Edge>();
            foreach (string vertex in adjacencyList.Keys)
            {
                Dictionary<string, int> adjacentVertices = adjacencyList[vertex];
                foreach (string adjacentVertex in adjacentVertices.Keys)
                {
                    edgeList.Add(new Edge(vertex, adjacentVertex, adjacentVertices[adjacentVertex]));
                }
            }


            Dictionary<string, int> distances = new Dictionary<string, int>();
            foreach (string node in adjacencyList.Keys)
            {
                distances.Add(node, int.MaxValue);
            }
            distances[src] = 0;


            for (int i = 1; i < distances.Count; i++)
            {
                for (int j = 0; j < edgeList.Count; j++)
                {
                    string u = edgeList[j].src;
                    string v = edgeList[j].dest;
                    int weight = edgeList[j].weight;
                    if (distances[u] != int.MaxValue && distances[u] + weight < distances[v])
                    {
                        distances[v] = distances[u] + weight;
                    }
                }
            }

            return distances;
        }

        public Dictionary<string, int> HasNegativeCycle(string src, ref HashSet<string> negativeVertices)
        {
            List<Edge> edgeList = new List<Edge>();
            foreach (string vertex in adjacencyList.Keys)
            {
                Dictionary<string, int> adjacentVertices = adjacencyList[vertex];
                foreach (string adjacentVertex in adjacentVertices.Keys)
                {
                    edgeList.Add(new Edge(vertex, adjacentVertex, adjacentVertices[adjacentVertex]));
                }
            }

            Dictionary<string, int> distances = new Dictionary<string, int>();
            foreach (string node in adjacencyList.Keys)
            {
                distances.Add(node, int.MaxValue);
            }
            distances[src] = 0;


            for (int i = 1; i < distances.Count; i++)
            {
                for (int j = 0; j < edgeList.Count; j++)
                {
                    string u = edgeList[j].src;
                    string v = edgeList[j].dest;
                    int weight = edgeList[j].weight;
                    if (distances[u] != int.MaxValue && distances[u] + weight < distances[v])
                    {
                        distances[v] = distances[u] + weight;
                    }
                }
            }

            for (int i = 0; i < edgeList.Count; i++)
            {
                string u = edgeList[i].src;
                string v = edgeList[i].dest;
                int weight = edgeList[i].weight;
                if (distances[u] != int.MaxValue && distances[u] + weight < distances[v])
                {
                    negativeVertices.Add(u);
                    negativeVertices.Add(v);
                }
            }

            return distances;
        }


        public int[,] Floyd()
        {
            int V = adjacencyList.Count;

            string[] vertices = new string[V];
            int i = 0, j, k;
            foreach(string vertex in adjacencyList.Keys)
            {
                vertices[i++] = vertex;
            }

            int[,] distances = new int[V, V];
            for(i = 0; i < V; i++)
            {
                for(j = 0; j < V; j++)
                {
                    if (adjacencyList[vertices[i]].ContainsKey(vertices[j]))
                    {
                        distances[i, j] = adjacencyList[vertices[i]][vertices[j]];
                    }
                    else if(i == j)
                    {
                        distances[i, j] = 0;
                    }
                    else
                    {
                        distances[i, j] = int.MaxValue;
                    }
                }
            }

            for(k = 0; k < V; k++)
            {
                for(i = 0; i < V; i++)
                {
                    for(j=0; j < V; j++)
                    {
                        if (distances[i, k] != int.MaxValue 
                            && distances[k,j] != int.MaxValue 
                            && distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                        }
                    }
                }
            }

            return distances;
        }

        public List<string> Bfs(Dictionary<string, Dictionary<string, int>> residualAdjacencyList, string src, string dest)
        {
            // Create a visited array and mark
            // all vertices as not visited
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach(string vertex in residualAdjacencyList.Keys)
            {
                visited.Add(vertex, false);
            }

            // Create a queue, enqueue source vertex and mark
            // source vertex as visited
            List<string> queue = new List<string>();
            queue.Add(src);
            visited[src] = true;


            Dictionary<string, List<string>> path = new Dictionary<string, List<string>>();
            path.Add(src, new List<string>());
            path[src].Add(src);

            // Standard BFS Loop
            while (queue.Count != 0)
            {
                string u = queue[0];
                queue.RemoveAt(0);

                foreach(KeyValuePair<string, int> edge in residualAdjacencyList[u]) 
                {
                    if (visited[edge.Key] == false
                        && edge.Value > 0)
                    {
                        // If we find a connection to the sink
                        // node, then there is no point in BFS
                        // anymore We just have to set its parent
                        // and can return true
                        if (edge.Key == dest)
                        {
                            List<string> result = new List<string>(path[u])
                            {
                                dest
                            };
                            
                            return result;
                        }

                        queue.Add(edge.Key);
                        path[edge.Key] = new List<string>(path[u])
                        {
                            edge.Key
                        };
                        visited[edge.Key] = true;
                    }
                }
            }

            // We didn't reach sink in BFS starting from source,
            // so return false
            return null;
        }

        public int FordFulkerson(string src, string dest)
        {
            Dictionary<string, Dictionary<string, int>> residualAdjacencyList = 
                new Dictionary<string, Dictionary<string, int>>(adjacencyList);

            int max_flow = 0; // There is no flow initially

            // Augment the flow while there is path from source
            // to sink

            List<string> path;
            while ((path = Bfs(residualAdjacencyList, src, dest)) != null)
            {
                // Find minimum residual capacity of the edges
                // along the path filled by BFS. Or we can say
                // find the maximum flow through the path found.
                int path_flow = int.MaxValue;
                for (int i = 1; i < path.Count; i++)
                {
                    string u = path[i - 1];
                    string v = path[i];
                    path_flow = Math.Min(path_flow, residualAdjacencyList[u][v]);
                }

                // update residual capacities of the edges and
                // reverse edges along the path
                for (int i = 1; i < path.Count; i++)
                {
                    string u = path[i - 1];
                    string v = path[i];
                    residualAdjacencyList[u][v] -= path_flow;
                    if (residualAdjacencyList[v].Keys.Contains(u))
                    {
                        residualAdjacencyList[v][u] += path_flow;
                    }
                    else
                    {
                        residualAdjacencyList[v].Add(u, path_flow);
                    }

                }

                // Add path flow to overall flow
                max_flow += path_flow;
            }

            // Return the overall flow
            return max_flow;
        }

        public Dictionary<string, Dictionary<string, int>> GetAdjacencyList()
        {
            return adjacencyList;
        }
    }
}
