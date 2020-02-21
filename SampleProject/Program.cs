
using Karzina.Common;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SampleProject
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log.Info("Main: **** Starting **********************************************************************************");
            SampleWorker worker = new SampleWorker
            {
                ConnectionString = "server=localhost;port=3306;database=prestashop;user=root;password="
            };
            worker.StartWorker();
            log.Info("Main: **** Ending **********************************************************************************");
        }
    }
}

