using System;
using System.Collections.Generic;

namespace simpleSalesman
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up the nodes in the graph.  A node represents a hypothetical city.
            // Each city has one feature that indicates if the city is on the perimeter of the network or not.
            const float PERIMETER_CITY = 1.0F;
            const float INTERIOR_CITY = 0.0F;
		
            // Use an adjacency list to represent the connections to the nodes in the graph.
            Graph graph = new Graph(10);
            graph.addEdge(0, new NodeData { Id=1, weight=3, features = new float[] {PERIMETER_CITY}});
            // Use the next line to show how being a perimeter city gives priority over having the least cost.
            // graph.addEdge(0, new NodeData { Id=3, weight=2, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(0, new NodeData { Id=3, weight=2, features = new float[] {INTERIOR_CITY}});
            graph.addEdge(0, new NodeData { Id=6, weight=4, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(1, new NodeData { Id=5, weight=3, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(5, new NodeData { Id=2, weight=2, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(2, new NodeData { Id=3, weight=5, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(3, new NodeData { Id=4, weight=2, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(6, new NodeData { Id=4, weight=6, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(4, new NodeData { Id=7, weight=3, features = new float[] {PERIMETER_CITY}});
            graph.addEdge(6, new NodeData { Id=8, weight=9, features = new float[] {PERIMETER_CITY}});
				
            // Find the path through the network with the least cost. 
            graph.TravelingSalesman(new NodeData { Id=0, weight=0, features = new float[] {PERIMETER_CITY}});
            Console.WriteLine();
        }
    }

    // Each node has an id, weight and an array of features.
    struct NodeData
    {
        public int Id { get; set; }
        public int weight { get; set; }
        public float[] features { get; set; }
    }

    class Graph
    {
        const float PERIMETER_CITY = 1.0F;
        const float INTERIOR_CITY = 0.0F;
        const int DESIRABLE = 0;
        const int NOT_DESIRABLE = 1;
        const bool AVOID_INTERIOR = true;
        const bool INCLUDE_INTERIOR = false;

	    private int numberOfNodes;
	    private List<NodeData>[] adj;

	    public Graph(int maxNum)
	    {
    		numberOfNodes = maxNum;
	    	adj = new List<NodeData>[maxNum];
		    for (int i = 0; i < maxNum; i++)
			    adj[i] = new List<NodeData>();
	    }

	    public void addEdge(int vertex, NodeData adjacentNode)
	    {
		    adj[vertex].Add(adjacentNode);
	    }

        // Find the path through the network with the least cost. 
        public void TravelingSalesman(NodeData start)
        {            
    	    bool[] visited = new bool[numberOfNodes];
		    for (int i = 0; i < numberOfNodes; i++)
			    visited[i] = false;
            
            // Follow the path with the least cost, but stay close to the perimeter.
            FollowingMinimimWeights(start, ref visited, AVOID_INTERIOR);

            // Display any unvisited nodes.
            bool firstTime = true;
            for (int i = 0; i < numberOfNodes; i++)
            {
                // Start at the first unvisited node and display any unvisited nodes that is is connected to.
                if (visited[i] == false && firstTime == true && i != 0)
                {
                    Console.Write("{0} ", i);
                    NodeData firstUnvisitedNode = new NodeData { Id=i, weight=0, features = new float[] {PERIMETER_CITY}};
                    FollowingMinimimWeights(firstUnvisitedNode, ref visited, INCLUDE_INTERIOR);
                    firstTime = false;
                }
            }
            Console.Write("{0} ", start.Id);
        }

        // Follow the path with the least cost, but stay close to the perimeter.
        public void FollowingMinimimWeights(NodeData sourceNode, ref bool[] visited, bool avoidInterior)
        {
            // base case
            if (adj[sourceNode.Id].Count == 0 || visited[sourceNode.Id] == true)
            {
                visited[sourceNode.Id] = true;
                return;
            }

            // initialize variables
            NodeData adjacent = new NodeData { Id=0, weight=0, features = new float[] {PERIMETER_CITY}};
            NodeData minNode = new NodeData { Id=0, weight=0, features = new float[] {PERIMETER_CITY}};
            int minWeight = int.MaxValue;

            // Examine each adjacent node to find the node with the least cost.
            // However, choose a node on the perimeter of the network over
            // a node that may have a lower cost.
            bool foundUnvisitedNodes = false;
	    for (int i = 0; i < adj[sourceNode.Id].Count; i++)
	    {
                adjacent = adj[sourceNode.Id][i];
	    	if (!visited[adjacent.Id])
		{
                    foundUnvisitedNodes = true;
                    // Avoid undesirable cities during the traversal.
                    int category = FindCategory(adjacent.features);
                    if (category != DESIRABLE && avoidInterior)
                        continue;
                    if (adjacent.weight < minWeight)
                    {
                        visited[sourceNode.Id] = true;
                        minWeight = adjacent.weight;
                        minNode = adj[sourceNode.Id][i];
                    }
		}
	    }    
            // Only display nodes that were visited
            if (foundUnvisitedNodes == true)
                Console.Write("{0}:{1} ", minNode.Id, minNode.weight);
		
            // If no nodes were visited, then display the sourceNode
            if (foundUnvisitedNodes == false)
                Console.Write("{0}:{1} ", sourceNode.Id, sourceNode.weight);
		
            FollowingMinimimWeights(minNode, ref visited, avoidInterior);        
        }

        // A city on the perimeter of the network is desirable over a city in the 
        // interior of the network.
        public int FindCategory(float[] features)
        {
            int Category = -1;

            if (features[0] == PERIMETER_CITY)
                Category = DESIRABLE;
            else
                Category = NOT_DESIRABLE;          

            return Category;
        }   

    }  
}
