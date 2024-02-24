using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetEx
{
    public class Program
    {
        //connection string DESKTOP-II4I5AG\SQLEXPRESS
        //database = MinionsDB

        const string connectionString = "Server=DESKTOP-II4I5AG\\SQLEXPRESS;Database=MinionsDB;Integrated Security=true;TrustServerCertificate=True";
       static SqlConnection? sqlConnection;
        public static async Task Main(string[]args)
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string minionInfoRaw = Console.ReadLine();
                string villainInfoRaw = Console.ReadLine();

                string minionInfo = minionInfoRaw.Substring(minionInfoRaw.IndexOf(":") + 1).Trim();
                string villainName = villainInfoRaw.Substring(villainInfoRaw.IndexOf(":") + 1).Trim();

                AddMinions(minionInfo, villainName);
            }
            finally
            {
                sqlConnection?.Dispose(); 
            }

            //Villian names
            //using SqlCommand getVilliansCommand = new SqlCommand(SqlQueries.getVilliansWithNumberOfMinions,sqlConnection);

            //using SqlDataReader dataReader = getVilliansCommand.ExecuteReader();
            //while(dataReader.Read())           
            // Console.WriteLine($"{dataReader[ "Name"]} - {dataReader[ "TotalMinions"]}");
            //await GetOrderMinionsByVillianId(1);


        }
        //3. Minion Names exercise
        static async Task GetOrderMinionsByVillianId(int id)
        {
            SqlCommand command = new SqlCommand(SqlQueries.getVilliansById, sqlConnection);
            command.Parameters.AddWithValue(@"Id", id);
            var result = await command.ExecuteScalarAsync();
            if(result == null)
            {
                await Console.Out.WriteLineAsync($"No villian with ID {id} exists in the database ");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Villain: {result}");

                using SqlCommand commandGetMinionData = new SqlCommand(SqlQueries.getOrderedVilliansByMinionsId, sqlConnection);
                commandGetMinionData.Parameters.AddWithValue(@"Id", id);

                var minionsReader = await commandGetMinionData.ExecuteReaderAsync();
                while(await minionsReader.ReadAsync())
                {
                    await Console.Out.WriteLineAsync($"{minionsReader["RowNum"]}. " + $"{minionsReader["Name"]} {minionsReader["Age"]}");
                }
            }

        }
        //4.
        static async Task AddMinions(string minionInfo,string villianName)
        {

            string[] minionData = minionInfo.Split(" ");
            string minionName = minionData[0];
            int minionAge = int.Parse(minionData[1]);
            string minionTown = minionData[2];
            
                SqlCommand cmdGetTownId = new SqlCommand(SqlQueries.getTownByName, sqlConnection);
                cmdGetTownId.Parameters.AddWithValue("@townName", minionTown);

                var townResult = await cmdGetTownId.ExecuteScalarAsync();

                int townId = -1;
                if (townResult == null)
                {
                    SqlCommand createTown = new SqlCommand(SqlQueries.InsertNewTown, sqlConnection);
                    createTown.Parameters.AddWithValue("@townName", minionTown);
                    townId = Convert.ToInt32(await createTown.ExecuteScalarAsync());
                    await Console.Out.WriteLineAsync($"Town {minionTown} was added to the database");
                }
                else
                {
                    townId = (int)townResult;
                }

                SqlCommand cmdGetVillain = new SqlCommand(SqlQueries.GetVillainName, sqlConnection);
                var villainResult = await cmdGetVillain.ExecuteScalarAsync();

                int villainId = -1;
                if (villainResult == null)
                {
                    SqlCommand insertNewVillain = new SqlCommand(SqlQueries.InsertNewVillain, sqlConnection);
                    insertNewVillain.Parameters.AddWithValue("@villainName", villianName);
                    insertNewVillain.Parameters.AddWithValue("@evilnessFactorId", 4);
                    villainId = Convert.ToInt32(await insertNewVillain.ExecuteScalarAsync());
                    await Console.Out.WriteLineAsync($"Villain {villianName} was added to the database");
                }
                else
                {
                    villainId = (int)villainResult;
                }

                SqlCommand insertMinion = new SqlCommand(SqlQueries.InsertNewMinion, sqlConnection);
                insertMinion.Parameters.AddWithValue("@minionName", minionName);
                insertMinion.Parameters.AddWithValue("@minionAge", minionAge);
                insertMinion.Parameters.AddWithValue("@townId", townId);
                await Console.Out.WriteLineAsync($"Minion {minionName} was inserted to database");

                int minionId = Convert.ToInt32(await insertMinion.ExecuteScalarAsync());

                SqlCommand insertMinionVillian = new SqlCommand(SqlQueries.InsertMinionsVilliansTable, sqlConnection);
                await insertMinionVillian.ExecuteNonQueryAsync();
                await Console.Out.WriteLineAsync($"Successfully added minion {minionName} as a servant to villain {villianName}");

        }
    }
}
