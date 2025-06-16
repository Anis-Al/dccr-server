using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class Wilaya : BaseTG
    {
        public List<Lieu> wilayas { get; set; }
        public List<Agence> agences { get; set; }
    }
}
