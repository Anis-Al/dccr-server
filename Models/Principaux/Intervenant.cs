using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DCCR_SERVER.Models.Principaux
{
    public class Intervenant
    {
        public  string cle { get; set; }

        public  string type_cle { get; set; }
        public string? nif { get; set; }
        public int cli { get; set; }
        public string? rib { get; set; }

        public List<Garantie>? garanties_intervenant {  get; set; } 
        public List<IntervenantCrédit> intervenant_credits { get; set; }

    }
}
