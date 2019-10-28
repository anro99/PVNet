using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVDataSampler
{
    static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] a_args)
        {
            logger.Info("Arguments: {Arguments}", string.Concat(a_args));
            if (StartAsApp(a_args))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);

                var mainForm = new MainForm();
                Application.Run(mainForm);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
            NLog.LogManager.Shutdown();
        }


        private static bool StartAsApp(string[] a_args)
        {
            if (a_args.Length == 0)
                return false;
            return string.Compare(a_args[0], "-app", StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
