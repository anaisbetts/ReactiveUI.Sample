using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Routing;

namespace ReactiveUI.Sample.ViewModels
{
    public class AppViewModel : ReactiveObject, IScreen
    {
        public AppViewModel()
        {
            Router = new RoutingState();
        }

        public RoutingState Router { get; protected set; }
    }
}
