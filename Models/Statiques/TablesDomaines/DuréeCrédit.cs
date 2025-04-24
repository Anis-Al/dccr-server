using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class DuréeCrédit : BaseTG
    {
        
        public List<Crédit> credits_duree_initiale { get; set; }
        public List<Crédit> credits_duree_restante { get; set; }
      
    }
}
