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
        private static void ChangeToSingle()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Single;
        }

        private static void ChangeToMultiple()
        {
            var hostService = (ServiceHost)OperationContext.Current.Host;
            var behavior = hostService.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.ConcurrencyMode = ConcurrencyMode.Multiple;
        }
    }
}
