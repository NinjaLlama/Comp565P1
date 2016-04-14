/*  
    Copyright (C) 2016 G. Michael Barnes
 
    The file NavNode.cs is part of AGMGSKv7 a port and update of AGXNASKv6 from
    MonoGames 3.2 to MonoGames 3.4  

    AGMGSKv7 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//#if MONOGAMES //  true, build for MonoGames
//   using Microsoft.Xna.Framework.Storage; 
//#endif
#endregion

namespace Project1
{

    /// <summary>
    /// A WayPoint or Marker to be used in path following or path finding.
    /// Four types of WAYPOINT:
    /// <list type="number"> WAYPOINT, a navigatable terrain vertex </list>
    /// <list type="number"> PATH, a node in a path (could be the result of A*) </list>
    /// <list type="number"> OPEN, a possible node to follow in an A*path</list>
    /// <list type="number"> CLOSED, a node that has been evaluated by A* </list>

    class NavGraph : NavNode
    {
        private NavNode root;
        private int spacing = 150;
        private Stage stage;
        Dictionary<String, NavNode> graph;


        public NavGraph()
        {
            
        }

        public NavGraph(Stage theStage)
        {
            graph = new Dictionary<String, NavNode>();
            stage = theStage;
            //stage = theStage;
            //x = anX;
            //z = aZ;
            //name = string.Format("{0}::{1}", x, z);
            //nodeType = NavGraphNodeType.VERTEX;
            //node = new NavNode(new Vector3(x * spacing, stage.Terrain.surfaceHeight(x, z), z * spacing), NavNode.NavNodeEnum.WAYPOINT);
        }

       

        // Example key for graph based on (x,z)
        private String skey(int x, int z)
        {
            return String.Format("{0} {1}/n", x, z);
        }

        // Use "this" 2D indexer property to access graph like an array
        // A wrapper for Dictionary<K,V>'s Item property
        public NavNode this[int x, int z]
        {
            get
            {
                NavNode node = null;
                try
                {
                    node = graph[skey(x, z)];
                    return node;
                }
                catch (KeyNotFoundException) { return node; }
            }
            set { graph[skey(x, z)] = value; }
        }

        // Example of foreach usage
        //		KeyValuePair<K,V>  	Dictionary's internal type
        //		*.Value	Dictionary's property for stored values
        private string visit()
        {
            string output = "";
            foreach (KeyValuePair<String, NavNode> item in graph)
                output = output + item.Key;
            return output;
        }

        // This example could have been written with TryGetValue()
        private void visitNode(int x, int z)
        {
            NavNode node = this[x, z];
            Console.Write("graph[{0}, {1}] == ", x, z);
            if (node == null)
                Console.WriteLine("null");
            else
                Console.WriteLine(node.ToString());
        }

        public void buildGraph()
        {
            List<NavNode> list = new List<NavNode>();
            for (int x = 0; x < 513; x = x + 21)
                for (int z = 0; z < 513; z = z + 21)
                {
                    NavNode node = new NavNode(new Vector3(x * spacing, stage.Terrain.surfaceHeight(x, z), z * spacing), NavNode.NavNodeEnum.WAYPOINT);
                    for (int i = 0; i < stage.Collidable.Count; i++)
                    {
                        if (accessible(node.Translation, stage.Collidable[i].Translation, stage.Collidable[i].ObjectBoundingSphereRadius))
                            list.Add(node);
                    }
                    graph.Add(skey(x, z), node);
                }
            Path path = new Path(stage, list, Path.PathType.SINGLE);
            stage.Components.Add(path);
        }

        public bool accessible(Vector3 nodePos, Vector3 objectPos, float radius)
        {
	        bool collision = false;
	        Vector3 position1 = nodePos;
	        Vector3 position2 = objectPos;

	        double distance = Math.Pow(Math.Abs(position1.X - position2.X), 2) + Math.Pow(Math.Abs(position1.Y - position2.Y), 2) + Math.Pow(Math.Abs(position1.Z - position2.Z), 2);

	        float sumRadius = (radius*radius);


	        if (distance < sumRadius)
		        collision = true;
	        else
		        collision = false;
	        return !collision;
        }
    }
}
