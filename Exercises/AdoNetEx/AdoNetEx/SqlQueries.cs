using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetEx
{
    public static class SqlQueries
    {
       public const string getVilliansWithNumberOfMinions =
                                @"COUNT(*) [TotalMinions]
                                FROM Villains AS v
                                JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
                                JOIN Minions AS m ON mv.MinionId = m.Id
                                GROUP BY v.[Name]
                                HAVING COUNT(*) > 3
                                ORDER BY COUNT(*) DESC";

        public const string getVilliansById = @"SELECT Name FROM Villains WHERE Id = @Id";
        public const string getOrderedVilliansByMinionsId = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";
        public const string getTownByName = @"SELECT Id FROM Towns WHERE Name = @townName";
        public const string InsertNewTown = @"INSERT INTO Towns ([Name]) OUTPUT inserted.id VALUES (@townName)";
        public const string GetVillainName = @"SELECT Id FROM Villains WHERE Name = @Name";
        public const string InsertNewVillain = @"INSERT INTO Villains ([Name], EvilnessFactorId) OUTPUT inserted.Id  VALUES (@villainName,@evilnessFactorId)";
        public const string InsertNewMinion = @"INSERT INTO Minions ([Name],Age,TownId) OUTPUT inserted.Id VALUES(@minionName,@minionAge,@townId)
";      public const string InsertMinionsVilliansTable = @"INSERT INTO MinionsVillains (MinionId,VillainId) VALUES(@minionId,@villianId)";
    }
}
