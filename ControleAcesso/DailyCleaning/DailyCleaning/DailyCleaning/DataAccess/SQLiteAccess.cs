using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyCleaning.DataAccess
{
    public class SQLiteAccess
    {
        private static IList<string> connections = null;

        public static IList<string> GetConnections()
        {
            try
            {
                connections = new List<string>();
                foreach (ConnectionStringSettings connectionStringSetting in ConfigurationManager.ConnectionStrings)
                {
                    if (connectionStringSetting.Name.StartsWith("VivoLoginDatabase"))
                    {
                        connections.Add(connectionStringSetting.ConnectionString);
                    }
                }
                return connections;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteQuery(string query)
        {
            try
            {
                foreach (string connectionString in SQLiteAccess.GetConnections())
                {
                    SQLiteConnection conn = new SQLiteConnection(connectionString);

                    using (conn)
                    {
                        conn.Open();
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            using (SQLiteCommand command = new SQLiteCommand(query, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
