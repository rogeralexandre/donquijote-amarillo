using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace adapter.rateio
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRateioAdapter" in both code and config file together.
    [ServiceContract]
    public interface IRateioAdapter
    {
        [OperationContract]
        void DoWork();
    }
}
