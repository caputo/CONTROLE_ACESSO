using DailyCleaning.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyCleaning
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var usersCleaning = new UsersClearning();
                Log.Log.LogInfo("Limpeza diária executada com sucesso!");
            }
            catch(Exception ex)
            {
                Log.Log.LogError(ex.Message, ex);

            }
        }
    }
}
