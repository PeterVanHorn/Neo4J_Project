using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanHorn_Neo4J_Final
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "letmein!")))
            using (var session = driver.Session())
            {
                int cyphercount = 1;

                //var prepCypher1 = @"CREATE CONSTRAINT workerIdConstraint FOR (worker:Worker) REQUIRE worker.id IS UNIQUE;";
                //var prepCypher2 = @"CREATE CONSTRAINT jobIdConstraint FOR (job:Job) REQUIRE job.id IS UNIQUE;";

                var cypher1 = @"
                LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_Job.csv' AS csvLine
                CREATE (:Job {id: toInteger(csvLine.id), name: csvLine.name, 
                        price: toInteger(csvLine.price), jobstart: csvLine.jobstart, 
                        timeline: toInteger(csvLine.timeline)});";

                var cypher2 = @"
                LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_Customer.csv' AS csvLine
                CREATE (:Customer {id: toInteger(csvLine.id),jobid: toInteger(csvLine.jobid),
                        firstname: csvLine.firstname,lastname: csvLine.lastname, city: csvLine.city,custphone: csvLine.custphone});";

                var cypher3 = @"
                LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_Worker.csv' AS csvLine
                CREATE (:Worker {id: toInteger(csvLine.id), firstname: csvLine.firstname,
                        jobid: toInteger(csvLine.jobid),
                        lastname: csvLine.lastname, jobrole: csvLine.jobrole, emcontact: csvLine.emcontact, 
                        emcontactphone: csvLine.emcontactphone});";

                var relCypher1 = @"LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_JobWorker.csv' AS csvLine 
                                    CALL {
                                    WITH csvLine
                                    MATCH (worker:Worker {id: toInteger(csvLine.workerid)}), (job:Job {id: toInteger(csvLine.jobid)})
                                    CREATE (worker)-[:WORKED_ON {jobworkerid: csvLine.jobworkerid}]->(job)
                                    } IN TRANSACTIONS OF 2 ROWS";

                var cypher4 = @"
                LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_Material.csv' AS csvLine
                CREATE (:Material {id: toInteger(csvLine.id),materialtype: csvLine.materialtype,description: csvLine.description});";

                var relCypher2 = @"LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_JobMaterial.csv' AS csvLine
                                    CALL {
                                    WITH csvLine
                                    MATCH (material:Material {id: toInteger(csvLine.materialid)}), (job:Job {id: toInteger(csvLine.jobid)})
                                    CREATE (material)-[:USED_ON {jobmaterialid: csvLine.jobmaterialid}]->(job)
                                    } IN TRANSACTIONS OF 2 ROWS";

                var cypher5 = @"
                LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_Equipment.csv' AS csvLine
                CREATE (:Equipment {id: toInteger(csvLine.id), equipmenttype: csvLine.equipmenttype, fuel: csvLine.fuel});";

                var relCypher3 = @"LOAD CSV WITH HEADERS FROM 'file:///LandscapeCo_JobEquipment.csv' AS csvLine
                                    CALL {
                                    WITH csvLine
                                    MATCH (equipment:Equipment {id: toInteger(csvLine.equipmentid)}), (job:Job {id: toInteger(csvLine.jobid)})
                                    CREATE (equipment)-[:USED_ON {jobequipmentid: csvLine.jobequipmentid}]->(job)
                                    } IN TRANSACTIONS OF 2 ROWS";

                List<string> cypherlist = new List<string>
                {
                    //prepCypher1,
                    //prepCypher2,
                    cypher1,
                    cypher2,
                    cypher3,
                    relCypher1,
                    cypher4,
                    relCypher2,
                    cypher5,
                    relCypher3
                };

                int proceedcount = 1;
                foreach (var str in cypherlist)
                {
                    session.Run(str);
                    cyphercount++;
                    Console.WriteLine("Cypher " + proceedcount + " complete: proceed?");
                    Console.ReadLine();
                    proceedcount++;
                }

                //job - customer relationship cypher
                for (int i = 1000; i < 1050; i++)
                {
                    var cjCypher = @"MATCH (c:Customer {jobid: " + i + "}),(j: Job { id: " + i + "})CREATE(c) - [:HAS_JOB]->(j); ";
                    var cjrelate = session.Run(cjCypher);
                }
                Console.ReadLine();
            }
        }
    }
}

