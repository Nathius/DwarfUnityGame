using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Models.AI.PathFinding
{
	public class PathFinder_AStar
	{
        //static veriables
	    private const int NUM_ORDS = 2;
        private const int NUM_DIRECTIONS = 4;
        private const int NUM_DIAGONALS = 8;
	    private static readonly int[,] m_diagonals = new int[NUM_DIAGONALS, NUM_ORDS]{
	        {0, 1}, //up
	        {1, 1}, //up right
	        {1, 0}, //right
	        {1, -1}, //down right
	        {0, -1}, //down
	        {-1, -1}, //down left
	        {-1, 0}, //left
	        {-1, 1}}; //up left
            
	    private static readonly int[,] m_directions = new int[NUM_DIRECTIONS, NUM_ORDS]{
	        {0, 1}, //up
	        {1, 0}, //right
	        {0, -1}, //down
	        {-1, 0}}; //left

        //class veriables
	    Tile m_startNode;
	    Tile m_endNode;
	    List<Tile> m_path;
	    List<PathingNode> m_openList;
	    List<PathingNode> m_closedList;
	    Tile[,] m_mapRef;
	    int M_MAP_WIDTH;
	    int M_MAP_HEIGHT;
	    bool m_includingDiagonals;
	    int m_iterationLimit;

        public PathFinder_AStar(int inWidth, int inHeight, Tile[,] inMap, bool inIncludingDiagonals)
        {
	        m_includingDiagonals = inIncludingDiagonals;
	        M_MAP_WIDTH = inWidth;
	        M_MAP_HEIGHT = inHeight;
	        m_mapRef = inMap;
	        m_iterationLimit = 100;
        }

        public void setIterationLimit(int inIterationLimit)
        {
	        m_iterationLimit = inIterationLimit;
        }

        public List<Tile> findPath(Tile inStart, Tile inEnd)
        {
            m_path = new List<Tile>();
            m_openList = new List<PathingNode>();
            m_closedList = new List<PathingNode>();

	        m_startNode = inStart;
	        m_endNode = inEnd;

	        PathingNode startNode = new PathingNode(inStart);
	        startNode.m_gScore = 0;
	        startNode.m_hScore = findHScore(startNode);
	        startNode.m_fScore = findFScore(startNode);
	        m_openList.Add(startNode);

	        PathingNode currentNode;
	        int iterations = 0;

            //Debug.Log("StartingPathing from:" + inStart.ToString() + " to:" + inEnd.ToString());

	        //while there are still nodes to look at
	        while((iterations < m_iterationLimit) && (m_openList.Count > 0))
	        {
		        iterations++;
                //Debug.Log(m_openList.Count + " " + m_closedList.Count);
		        //chose the next closest node to the end
		        currentNode = findClosestNode();

		        //if at end return path, else find next path step
		        if(currentNode.m_node == inEnd)
		        {
                    //Debug.Log("at end");
			        PathingNode tempNode = currentNode;

			        while(tempNode.m_parent != null)
			        {
                        //Debug.Log("build return path");
				        m_path.Add(tempNode.m_node);
				        tempNode = tempNode.m_parent;
			        }

			        return m_path;
		        }
		        else
		        {
			        //remove current node from the open list
			        bool found = false;
			        for(int i = 0; (i < m_openList.Count) && !found; i++)
			        {
				        if(m_openList[i] == currentNode)
				        {
                            //Debug.Log("remove current node from open list");
					        m_openList.RemoveAt(i);
					        found = true;
				        }
			        }

			        //add the current node to the closed list
			        m_closedList.Add(currentNode);

			        int xDif = 0;
			        int yDif = 0;
			        PathingNode tempNextPathNode = null;
			        //for each nabor of current, ether diagonally or only straight
			        if(m_includingDiagonals)
			        {
				        //check all diagonals
				        for(int i = 0; i < NUM_DIAGONALS; i++)
				        {
					        xDif = m_diagonals[i, 0];
					        yDif = m_diagonals[i, 1];
					        //if the next co-ord would be inside the map

					        bool search = true;
					        //check that the search does not try to access nodes outside the map
					        if((xDif == 1) && currentNode.m_node.Position.x >= (M_MAP_WIDTH - 1)) {search = false;}
					        if((xDif == -1) && currentNode.m_node.Position.x <= 0) {search = false;}
					        if((yDif == 1) && currentNode.m_node.Position.y >= (M_MAP_HEIGHT - 1)) {search = false;}
					        if((yDif == -1) && currentNode.m_node.Position.y <= 0) {search = false;}

					        if(search)
					        {
						        tempNextPathNode = new PathingNode(m_mapRef[((int)currentNode.m_node.Position.x) + xDif, ((int)currentNode.m_node.Position.y) + yDif]);
							        //m_mapRef[currentNode->getNode()->x + xDif][currentNode->getNode()->y + yDif]
						        look(tempNextPathNode, currentNode);
					        }

				        }
			        }
			        else
			        {
                        //Debug.Log("check adjacent nodes");
				        //only check straight adjectent squares
				        for(int i = 0; i < NUM_DIRECTIONS; i++)
				        {
					        xDif = m_directions[i, 0];
					        yDif = m_directions[i, 1];
					        //if the next co-ord would be inside the map
		
					        bool search = true;
					        //check that the search does not try to access nodes outside the map
					        if((xDif == 1) && currentNode.m_node.Position.x >= (M_MAP_WIDTH - 1)) {search = false;}
					        if((xDif == -1) && currentNode.m_node.Position.x <= 0) {search = false;}
					        if((yDif == 1) && currentNode.m_node.Position.y >= (M_MAP_HEIGHT - 1)) {search = false;}
					        if((yDif == -1) && currentNode.m_node.Position.y <= 0) {search = false;}

                            //Debug.Log("should search next: " + search.ToString());
					        if(search)
					        {
						        tempNextPathNode = new PathingNode(m_mapRef[((int)currentNode.m_node.Position.x) + xDif, ((int)currentNode.m_node.Position.y) + yDif]);
							        //m_mapRef[currentNode->getNode()->x + xDif][currentNode->getNode()->y + yDif]
						        look(tempNextPathNode, currentNode);
					        }
				        }
			        }
		        }

		        //displayMap();
		        //system("PAUSE");
	        }
	        //deletePath();
	        //end node not found
	        return null;
        }

        //Filled
        private int findFScore(PathingNode inPathNode)
        {
	        //returns dist to node from start, plus guess distance from node to end
	        return (int)(inPathNode.m_gScore + findHScore(inPathNode));
        }

        //Filled
        private float findHScore(PathingNode inPathNode)
        {
            //WARNING cast losing precscision, should round insted??
	        int xDist = (int)Math.Abs(inPathNode.m_node.Position.x - m_endNode.Position.x);
            int yDist = (int)Math.Abs(inPathNode.m_node.Position.y - m_endNode.Position.y);

	        //pythagorus a^2 + b^2 = c^2
	        double hypotDist = (xDist * xDist) + (yDist * yDist);
	        hypotDist = Math.Sqrt(hypotDist);

	        //round distance to nerest whole number
	        //NOTE seems to give less organic results than not rounding.
	        //ie moving more left and right, than diagonally
	        //hypotDist = floor(hypotDist + 0.5);

	        return (float)hypotDist;
        }

        //filled
        private PathingNode findClosestNode()
        {
	        int lowestNode = -1;
	        float lowestFScore = -1;
	        float tempFScore = 0;

	        //for each node in the open list, find the node that is closest to the end
	        for(int i = 0; i < m_openList.Count(); i++)
	        {
		        //tmporary total path length guess for i^th node
		        tempFScore = findFScore(m_openList[i]);

		        //set lowest to the i^th node if it is closer to the end
		        if((tempFScore < lowestFScore) || (lowestFScore == -1))
		        {
			        lowestNode = i;
			        lowestFScore = tempFScore;
		        }
	        }
	        return m_openList[lowestNode];

        }

        //filled
        private bool inOpenList(PathingNode inPathNode)
        {
	        for(int i = 0; (i < m_openList.Count); i++)
	        {
                if ((m_openList[i].m_node.Position.x == inPathNode.m_node.Position.x) &&
                    (m_openList[i].m_node.Position.y == inPathNode.m_node.Position.y))
		        {
			        return true;
		        }
	        }
	        return false;
        }

        //filled
        private bool inClosedList(PathingNode inPathNode)
        {
	        for(int i = 0; (i < m_closedList.Count); i++)
	        {
                if ((m_closedList[i].m_node.Position.x == inPathNode.m_node.Position.x) &&
                    (m_closedList[i].m_node.Position.y == inPathNode.m_node.Position.y))
		        {
			        return true;
		        }
	        }
	        return false;
        }

        //filled, ?optamised?
        private void look(PathingNode inPathNode, PathingNode inPrevPathNode)
        {
            //Debug.Log("looking at next node");
	        //bool used = false;
	        //if node is not in closed list, and can be traversed
            //Debug.Log("isInClosedList: " + inClosedList(inPathNode).ToString() + " has cost: " + inPathNode.m_node.Cost);
	        if(!inClosedList(inPathNode) && (inPathNode.m_node.Cost > 0))
	        {
                //Debug.Log("node not in closed list");
		        // if the node is not in the open list, or is but can be reached faster via the new path
		        if(!inOpenList(inPathNode) || (inOpenList(inPathNode) && (inPrevPathNode.m_gScore + inPathNode.m_node.Cost < inPathNode.m_gScore)))
		        {
                    //Debug.Log("node can be looked at");
			        //cout << "Look() newPathLenght: " << (inPrevNode->gScore + inNode->cost) << " Look() oldPathLength: " << inNode->gScore << endl;
			        if(!inOpenList(inPathNode))
			        {
                        //Debug.Log("add looked at node to open list");
				        //add it to the list only if it is not already on it
				        m_openList.Add(inPathNode);
				        //used = true;
			        }
			        //then update it's distance scores based on the new path data
			        inPathNode.m_gScore = (inPrevPathNode.m_gScore + inPathNode.m_node.Cost);
			        inPathNode.m_hScore = (findHScore(inPathNode));
			        inPathNode.m_fScore = (int)(inPathNode.m_gScore + inPathNode.m_hScore);
			        inPathNode.m_parent = inPrevPathNode;
		        }
		
	        }
            //if(!used)
            //{
            //    delete inPathNode;
            //}

        }

        //probably not needed in a unity application
        private void displayMap()
        {
	        //cout << endl;
	        //PathingNode tempPathNode;
	        bool placed = false;
	        for(int y = 0; y < M_MAP_HEIGHT; y++)
	        {
		        for(int x = 0; x < M_MAP_WIDTH; x++)
		        {
			        if(m_mapRef[x, y].Cost > 0)
			        {
				        placed = false;
				
				        for(int i = 0; i < m_closedList.Count && !placed; i++)
				        {
					        if(m_closedList[i].m_node.Position.x == x && m_closedList[i].m_node.Position.y == y )
					        {
						        //int temp = (int)m_closedList[i].m_hScore;
						        //cout << temp;
                                //if(m_closedList[i].m_hScore < 10)
                                //{
                                //    //cout << " ";
                                //}
						        placed = true;
					        }
					
				        }
				        for(int i = 0; i < m_openList.Count && !placed; i++)
				        {
					        if(m_openList[i].m_node.Position.x == x && m_openList[i].m_node.Position.y == y )
					        {
						        //int temp = (int)m_openList[i].m_hScore;
                                //cout << temp;
                                //if(m_openList[i]->getHScore() < 10)
                                //{
                                //    cout << " ";
                                //}
						        placed = true;
					        }
					
				        }
                        //if(!placed)
                        //{
                        //    cout << ". " ;
                        //}
				
				        //cout << tempPathNode->getHScore();
			        }
                    //else
                    //{
                    //    cout << char(254);
                    //}
			        //cout << " ";
		        }
		        //cout << endl;
	        }
	        //cout << endl;
        }


	}
}
