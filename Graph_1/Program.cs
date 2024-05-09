using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Graph_1.Graph;

namespace Graph_1.Graph
{
    internal class Program
    {
        IGraph graph;

        void Run()
        {
            Console.WriteLine("Welcome to Graph Inteface!");
            Console.WriteLine("To get help type: help");
            for(; ; )
            {
                string command = GetCommand();
                string[] fields = command.Split(' ');
                if (fields[0].StartsWith("q"))
                {
                    break;
                }
                if (fields[0] == "read")
                {
                    ReadGraph(fields);
                }
                if (fields[0] == "save")
                {
                    SaveGraph(fields);
                }
                if (fields[0] == "show")
                {
                    ShowGraph();
                }
                if (fields[0] == "addVertex")
                {
                    AddVertex(fields);
                }
                if (fields[0] == "removeVertex")
                {
                    RemoveVertex(fields);
                }
                if (fields[0] == "addEdge")
                {
                    AddEdge(fields);
                }
                if (fields[0] == "removeEdge")
                {
                    RemoveEdge(fields);
                }
                if (fields[0] == "newGraph")
                {
                    MakeNewGraph(fields);
                }
                if (fields[0] == "help")
                {
                    Help();
                }
                if (fields[0] == "copyGraph")
                {
                    CopyGraph();
                }
                if (fields[0] == "2_task1a")
                {
                    Task2_1a();
                }
                if (fields[0] == "3_task1a")
                {
                    Task3_1a(fields);
                }
                if (fields[0] == "4_task1b")
                {
                    Task4_1b(fields);
                }
                if (fields[0] == "5_task2_DFS")
                {
                    Task5_2_DFS(fields);
                }
                if (fields[0] == "6_task2_BFS")
                {
                    Task6_2_BFS(fields);
                }
                if (fields[0] == "7_task")
                {
                    Task7(fields);
                }
                if (fields[0] == "8_task4a")
                {
                    Task8(fields);
                }
                if (fields[0] == "9_task4b")
                {
                    Task9(fields);
                }
                if (fields[0] == "10_task4c")
                {
                    Task10(fields);
                }
                if (fields[0] == "11_task")
                {
                    Task11(fields);
                }
            }
        }

        private void Task11(string[] fields)
        {
            Console.WriteLine("Решить задачу на нахождение максимального потока любым алгоритмом.");
            ReadGraph(fields);

            Console.WriteLine("Исходный граф:");
            Console.WriteLine(graph.ToString());

            List<string> path = graph.Bfs(graph.GetAdjacencyList(), fields[2], fields[3]);
            foreach(string v in path)
            {
                Console.Write("{0},", v);
            }
            Console.WriteLine();

            Console.WriteLine("Max flow {0}", graph.FordFulkerson(fields[2], fields[3]));
        }

        private void Task10(string[] fields)
        {
            Console.WriteLine("9. Вывести длины кратчайших путей для всех пар вершин. В графе возможны отрицательные циклы.");
            ReadGraph(fields);

            List<string> vertices = graph.GetVertices();
            int[,] result = new int[vertices.Count, vertices.Count];
            HashSet<string> negativeVertices = new HashSet<string>();
            for (int i = 0; i < vertices.Count; i++)
            {
               
                Dictionary<string, int> distances = graph.BellmanFord(vertices[i]);
                for(int j = 0; j < vertices.Count; j++)
                {
                    result[i, j] = distances[vertices[j]];
                }

            }

            PrintDistances(graph, result, ref negativeVertices);
            


            //if (!graph.HasNegativeCycle(graph.GetVertices()[0]))
            //{
            //    int[,] distances = graph.Floyd();
            //    PrintFloyd(graph, distances);
            //}
            //else
            //{
            //    Console.WriteLine("Graph has negative Cycle!");
            //}
            
        }

        private void PrintDistances(IGraph graph, int[,] distances, ref HashSet<string> negativeVertices)
        {
            int V = distances.GetLength(0);

            Console.Write("\t");
            foreach (string vertex in graph.GetVertices())
            {
                Console.Write("{0}\t", vertex);
            }
            Console.WriteLine();

            for (int i = 0; i < V; i++)
            {
                Console.Write("{0}\t", graph.GetVertices()[i]);
                for (int j = 0; j < V; j++)
                {
                    //if (negativeVertices.Contains(graph.GetVertices()[i]) ||
                    //    negativeVertices.Contains(graph.GetVertices()[j]))
                    //{
                    //    Console.Write("Neg\t");
                    //}
                    //else
                    if (distances[i,j] == int.MaxValue)
                    {
                        Console.Write("Max\t");
                    }
                    else
                    {
                        Console.Write("{0}\t", distances[i, j]);
                    }
                    
                }
                Console.WriteLine();
            }
        }

        private void PrintFloyd(IGraph graph, int[,] distances)
        {
            int V = distances.GetLength(0);

            Console.Write("\t");
            foreach(string vertex in graph.GetVertices()) 
            {
                Console.Write("{0}\t", vertex);
            }
            Console.WriteLine();

            for (int i = 0; i < V; i++)
            {
                Console.Write("{0}\t", graph.GetVertices()[i]);
                for (int j = 0; j < V; j++)
                {
                    Console.Write("{0}\t", distances[i,j]);
                }
                Console.WriteLine();
            }
        }

        private void Task9(string[] fields)
        {
            Console.WriteLine("3. Определить, есть ли в графе вершина, каждая из минимальных стоимостей пути от которой до остальных не превосходит N.");
            ReadGraph(fields);

            string resultVertex = null;
            int resultVertexIndex = -1;
            int[,] distances = graph.Floyd();
            for(int i = 0; i < distances.GetLength(0); i++)
            {
                bool resultFound = true;
                for (int j = 0; j < distances.GetLength(1); j++)
                {
                    if (distances[i,j] >= int.Parse(fields[2]))
                    {
                        resultFound = false;
                        break;
                    }

                }
                if (resultFound)
                {
                    resultVertex = graph.GetVertices()[i];
                    resultVertexIndex = i;
                    break;
                }
            }

            if(resultVertex != null)
            {
                Console.WriteLine("Result Vertex: {0}", resultVertex);
                Console.WriteLine("Distance:");
                for (int i = 0; i < distances.GetLength(1); i++)
                {
                    Console.WriteLine("{0} : {1} ", graph.GetVertices()[i], distances[resultVertexIndex, i]);
                }
                Console.WriteLine();
            }
            else 
            {
                Console.WriteLine("No node found!");
            }

            PrintFloyd(graph, distances);
        }

        private void Task8(string[] fields)
        {
            Console.WriteLine("7. N-периферией для вершины называется множество вершин, расстояние от которых до заданной вершины больше N. Определить N-периферию для заданной вершины графа.");
            ReadGraph(fields);
            Dictionary<string, int> distances = new Dictionary<string, int>();

            foreach (string vertex in graph.GetVertices())
            {
                Dictionary<string, int> dist = graph.Dijkstra(vertex);
                distances[vertex] = dist[fields[2]];
            }

            Console.WriteLine("Distances:");
            foreach (string vertex in distances.Keys) 
            {
                Console.WriteLine("{0} {1}", vertex, distances[vertex]);
            }
            Console.WriteLine();

            Console.WriteLine("Result:");
            foreach (string vertex in distances.Keys)
            {
                if (distances[vertex] > int.Parse(fields[3]))
                {
                    Console.WriteLine("{0} {1}", vertex, distances[vertex]);
                }
            }
        }

        private void Task7(string[] fields)
        {
            Console.WriteLine("Краскал. Дан взвешенный неориентированный граф из N вершин и M ребер. Найти в нем каркас минимального веса");
            ReadGraph(fields);

            Console.WriteLine("Исходный граф:");
            Console.WriteLine(graph.ToString());

            Tuple<IGraph, int> result = graph.Kruskal();

            Console.WriteLine("resulting weght {0}", result.Item2);
            Console.WriteLine("MST graph:");
            Console.WriteLine(result.Item1.ToString());

        }

        private void Task6_2_BFS(string[] fields)
        {
            Console.WriteLine("29. Распечатать самый короткий (по числу дуг) из путей от u до остальных вершин.");
            IGraph graph1 = new Graph(fields[1]);
            Dictionary<string, List<string>> paths = graph1.GetShortestPaths(fields[2]);


            Console.WriteLine("Исходный граф:");
            Console.WriteLine(graph1.ToString());
            Console.WriteLine("Все пути от вершины {0}", fields[2]);
            PrintAllPaths(paths);
            
            foreach(string vertex in graph1.GetVertices())
            {
                if (!paths.Keys.Contains(vertex))
                {
                    Console.WriteLine("Нет пути из вершины {0} в {1}", fields[2], vertex);
                }
            }
        }

        private void PrintAllPaths(Dictionary<string, List<string>> allPaths)
        {
            foreach (string vertex in allPaths.Keys)
            {
                foreach (string node in allPaths[vertex])
                {
                    Console.Write(node);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        private void Task5_2_DFS(string[] fields)
        {
            Console.WriteLine("6.Найти связные компоненты графа.");
            IGraph graph1 = new Graph(fields[1]);                   // создаем экземпляр графа

            Console.WriteLine("Исходный граф:");
            Console.WriteLine(graph1.ToString());

            Console.WriteLine("Связные компоненты графа:");
            List<IGraph> components = graph1.GetAdjacencyComponents(); // Получаем список компонент связности 
            foreach (var linkedComponent in components)
            {
                Console.WriteLine(linkedComponent.ToString());  // выводим результат
            }
        }

        private void Task4_1b(string[] fields)
        {
            Console.WriteLine("10. Построить орграф, являющийся симметрической разностью по дугам двух заданных орграфов" +
                "\n(множество вершин получается объединением вершин исходных орграфов)");
            IGraph graph1 = new Graph(fields[1]); // создаем 2 экземпляра класса невзвешенного графа
            IGraph graph2 = new Graph(fields[2]);
            
            List<string> vertices1 = graph1.GetVertices();   // получаем списки вершин графов
            List<string> vertices2 = graph2.GetVertices();

            graph = new Graph(true);                        // создаем и заполняем граф вершинами из 2ух исходных графов
            foreach(var vertex in vertices1)
            {
                graph.AddVertex(vertex);
            }
            foreach(var vertex in vertices2)
            {
                if (!vertices1.Contains(vertex))
                {
                    graph.AddVertex(vertex);
                }
            }

            foreach (var vertex in graph.GetVertices())        // получаем списки смежности для каждой вершины в результирующем графе
            {
                List<string> adjacentVertices1 = graph1.GetAdjacentVertices(vertex); 
                List<string> adjacentVertices2 = graph2.GetAdjacentVertices(vertex);
                foreach(var adjacentVetex in adjacentVertices1)        
                {
                    if (!adjacentVertices2.Contains(adjacentVetex)) // если список смежных вершин второго графа не содержит вершины из первого, то добавляем ребро  
                    {
                        graph.AddEdge(vertex, adjacentVetex);
                    }                                       
                }

                foreach(var adjacentVertex in adjacentVertices2)   // аналогично в случае, если ребро из списка смежности второго графа содержится в списке смежности первого
                {
                    if (!adjacentVertices1.Contains(adjacentVertex))
                    {
                        graph.AddEdge(vertex, adjacentVertex);
                    }    
                }
            }
            Console.WriteLine("To see result input {show} command");
        }

        private void Task3_1a(string[] fields)
        {
            Console.WriteLine("14. Вывести все вершины орграфа, смежные с данной.");
            if (fields.Length == 2)
            {
                if (graph.IsDirected())
                {
                    Console.WriteLine("Result:");
                    List<string> adjacentVertices = graph.GetAdjacentVertices(fields[1]);
                    
                    if (adjacentVertices.Count > 0)
                    {
                        StringBuilder result = new StringBuilder();
                        foreach (string adjacentVertex in adjacentVertices)
                        {
                            result.Append(adjacentVertex);
                            result.Append(',');
                        }
                        Console.WriteLine(result.ToString().TrimEnd(','));
                    }
                    else
                    {
                        Console.WriteLine("No adjacent Vertices found!");
                    }
                }
                else
                {
                    Console.Error.WriteLine(string.Format("Graph should be directed!"));
                }
            }
            else
            {
                Console.Error.WriteLine(string.Format("No vertex input"));
            }
        }

        private void Task2_1a()
        {
            Console.WriteLine("6. Вывести все висячие вершины графа (степени 1)");
            if (!graph.IsDirected())
            {
                List<string> pendantVertices = graph.GetPendantVertices();
                Console.WriteLine("Result:");

                if (pendantVertices.Count > 0)
                {
                    StringBuilder result = new StringBuilder();
                    foreach (string pendantVertex in pendantVertices)
                    {
                        result.Append(pendantVertex);
                        result.Append(',');
                    }
                    Console.WriteLine(result.ToString().TrimEnd(','));
                }
                else
                {
                    Console.WriteLine("No pendant vertices found!");
                }
            }
            else
            {
                Console.WriteLine("Pendant vertices exist in undirected graph only!");
            }
        }

        // А нужно ли..
        private void CopyGraph()
        {
            Console.WriteLine("No implementation");           
        }

        private void Help()
        {
            using (StreamReader file = new StreamReader("D:/tmp/Graphs/Help.txt"))
            {
                string str;
                while ((str = file.ReadLine()) != null)
                {
                    Console.WriteLine(str);
                }
            }
        }

        private void MakeNewGraph(string[] fields)
        {
            if (fields.Length == 3)
            {
                if (fields[1] == "weighted") 
                {
                    graph = new WeightedGraph(fields[2] == "oriented");
                }
                else
                {
                    // отслеживание опечаток в комнде
                    if (fields[1] == "unweighted")
                    {
                        graph = new Graph(fields[2] == "oriented");
                    }
                    else
                    {
                        Console.Error.WriteLine("Typo in weight property");
                    }
                    
                }
            }
        }

        private void RemoveEdge(string[] fields)
        {
            if (graph == null)
            {
                Console.WriteLine("No graph");
                return;
            }
            if (fields.Length == 3)
            {
                try
                {
                    graph.RemoveEdge(fields[1], fields[2]);
                }
                catch (GraphException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        private void AddEdge(string[] fields)
        {
            if (graph == null)
            {
                Console.WriteLine("No graph");
                return;
            }

            if (graph.IsWeighted())
            {
                if (fields.Length == 4)
                {
                    try
                    {
                        graph.AddEdge(fields[1], fields[2], int.Parse(fields[3]));
                    }
                    catch (GraphException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
            else
            {
                if (fields.Length == 3)
                {
                    try
                    {
                        graph.AddEdge(fields[1], fields[2]);
                    }
                    catch (GraphException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
            
        }

        private void RemoveVertex(string[] fields)
        {
            if (fields.Length == 2)
            {
                try
                {
                    graph.RemoveVertex(fields[1]);
                }
                catch (GraphException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                    
            }
            else
            {
                Console.WriteLine("Vertex name required");
            }
        }

        string GetCommand()
        {
            Console.Write(">>> ");
            string command = Console.ReadLine();
            return command;
        }

        void ReadGraph(string[] fields)
        {
            if (fields.Length == 1)
            {
                Console.Error.WriteLine("No graph name input: read {file name}");
            }
            else
            {
                using (StreamReader sr = new StreamReader(fields[1]))
                {
                    string line = sr.ReadLine();
                    string[] graphType = line.Split(',');
                    if (graphType[0] == "1")
                    {
                        graph = new WeightedGraph(fields[1]);
                    }
                    else
                    {
                        graph = new Graph(fields[1]);
                    }
                }
                
            }
  
        }

        void SaveGraph(string[] fields)
        {
            if (fields.Length == 2)
            {
                graph.Save(fields[1]);
            }
            else
            {
                Console.WriteLine("File name required");
            }
        }

        void ShowGraph()
        {
            if (graph != null)
            {
                Console.WriteLine(graph.ToString());
            }
            else
            {
                Console.WriteLine("No graph");
            }
        }
        
        void AddVertex(string[] fields)
        {

            if (graph != null)
            {
                
                if (fields.Length == 2)
                {
                    try
                    {
                        graph.AddVertex(fields[1]);
                    }
                     catch (GraphException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Vertex name required");
                }
            }
            else
            {
                Console.WriteLine("No graph");
            }
            
        }


        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();


        }
    }
}