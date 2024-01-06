using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Utilities
{
    public static class HostBehaviorManager
    {
        public static void ChangeToSingle()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Single;
        }

        public static void ChangeToReentrant()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Reentrant;
        }
    }
}
