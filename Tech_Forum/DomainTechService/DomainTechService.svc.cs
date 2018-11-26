using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace DomainTech
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DomainTechService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DomainTechService.svc or DomainTechService.svc.cs at the Solution Explorer and start debugging.
    public class DomainTechService : IDomainTechService
    {
        public List<Domain_Table> GetDomainList()
        {
            using (DomainTechEntity dt = new DomainTechEntity())
            {
                List<Domain_Table> DomainList = dt.Domain_Table.ToList();
                return DomainList;
            }
               
        }

        public List<Technology_Table> GetTechforDomain(string domain)
        {
            using (DomainTechEntity dt = new DomainTechEntity())
            {
                int selecteddomain = Convert.ToInt32(domain);
                List<Technology_Table> TechnologyList = dt.Technology_Table.Where(x => x.did == selecteddomain).ToList();
                return TechnologyList;
            }
               


        }
        public void DoWork()
        {
        }
    }
}
