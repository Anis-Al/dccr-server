using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class Pays : BaseTG
    {
        public List<Lieu> pays { get; set; }
    }
}
