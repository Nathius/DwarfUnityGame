using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.AI.PathFinding
{
	public class PathingNode
	{
        public Tile m_node {get; set; }
	    public PathingNode m_parent {get; set; }
	    //pathfinding data
	    public int m_gScore {get; set; }
	    public float m_hScore { get; set; }
	    public int m_fScore {get; set; }

        public PathingNode(Tile inNode)
        {
	        m_node = inNode;
	        m_gScore = -1;
	        m_hScore = -1;
	        m_fScore = -1;
	        m_parent = null;
        }

        public PathingNode(Tile inNode, int inGScore, float inHScore, int inFScore)
        {
	        m_node = inNode;
	        m_gScore = inGScore;
	        m_hScore = inHScore;
	        m_fScore = inFScore;
        }
	}
}

