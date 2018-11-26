using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DomainTech
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDomainTechService" in both code and config file together.
    [ServiceContract]
    public interface IDomainTechService
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        List<Domain_Table> GetDomainList();

        [OperationContract]
        List<Technology_Table> GetTechforDomain(string domain);
    }

    [DataContract]
    public partial class Domain_Table
    {

    }

    [DataContract]
    public partial class Technology_Table
    {

    }
    /* public class Domain
     {
         public int did { get; set; }
         public string domain { get; set; }
     }*/


}
