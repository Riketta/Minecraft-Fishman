using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fishman
{
    class Program
    {
        private static readonly string BotName = "Minecraft Fishman";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LogManager.SetupLogger();
            logger.Info("{0} ver. {1}", BotName, Assembly.GetEntryAssembly().GetName().Version.ToString());
            logger.Info("Author Riketta. Feedback: rowneg@bk.ru / https://github.com/riketta");

            logger.Info("Ready to start fishing");
            logger.Info("Press \"Enter\" to start");
            Console.ReadLine();
            logger.Info("Starting fishing after 5 seconds delay");
            Bot bot = new Bot();
            bot.FishingLoop();
        }
    }
}
