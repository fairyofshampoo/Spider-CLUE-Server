using System.ServiceModel;

namespace GameService.Utilities
{
    /// <summary>
    /// Utility class for managing concurrency modes in a WCF service host.
    /// </summary>
    public static class HostBehaviorManager
    {
        /// <summary>
        /// Changes the concurrency mode of the current service host to Single.
        /// </summary>
        public static void ChangeToSingle()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Single;
        }

        /// <summary>
        /// Changes the concurrency mode of the current service host to Reentrant.
        /// </summary>
        public static void ChangeToReentrant()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Reentrant;
        }
    }
}