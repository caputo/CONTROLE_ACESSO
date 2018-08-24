using RequestConsolidate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestConsolidate
{
    class Program
    { 
        static void Main(string[] args)
        {
            try
            {
                var consolidate = new Consolidate(true);
                Log.Log.LogInfo("Requisições consolidadas com sucesso!");
            }
            catch (Exception ex)
            {
                Log.Log.LogError(ex.Message, ex);
            }
        }
    }
}
