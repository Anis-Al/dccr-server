using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class ActivitéCrédit : BaseTG
    {
        
        public List<Crédit> credits_type_activite { get; set; }
    }
}
