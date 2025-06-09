using DCCR_SERVER.Models.Statiques.TablesDomaines;

namespace DCCR_SERVER.Models.Principaux
{
    public class Lieu
    {
       public int id_lieu { get; set; }
        public string? code_agence { get; set; }
        public string code_wilaya { get; set; }
        public string code_pays { get; set; }

        public List<Crédit> credits { get; set; } 
        public Agence agence { get; set; }
        public Wilaya wilaya { get; set; }
        public Pays pays { get; set; }

    }
}
