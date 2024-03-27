using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace VanHorn_Database_Final
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "neo4j")))
            using (var session = driver.Session())
            {
                //create a new graph database using my landscapeco csv
                //session.Run("CREATE (a:Person2 {name:'Alfred', title:'King'})");
            }
        }
    }
}