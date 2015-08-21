using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleCycle.Svc.Dispatcher.Contract
{
    public interface ISmsDispatch : IDispatch
    {
        string To { get; set; }
        string From { get; set; }
        string Text { get; set; }
    }
}
