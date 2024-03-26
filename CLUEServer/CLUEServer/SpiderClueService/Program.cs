using System;
using System.ServiceModel;
using GameService.Utilities;

namespace SpiderClueService
{
    public static class Program
    {
        static void Main(string[] args)
        {
            LoggerManager logger = new LoggerManager(typeof(Program));
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(GameService.Services.GameService)))
                {
                    host.Open();
                    Console.WriteLine("Server is running");
                    Console.ReadLine();
                }
            }
            catch (AddressAccessDeniedException addressException)
            {
                logger.LogError(addressException);
                Console.ReadLine();
            }
            catch (CommunicationException communicationException)
            {
                logger.LogError(communicationException);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                Console.ReadLine();
            }
        }
    }
}
