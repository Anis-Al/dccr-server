using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class Agence : BaseTG
    {
        public string wilaya_code { get; set; }
        public Wilaya wilaya { get; set; }
        public List<Lieu> agences { get; set; }

    }
}
