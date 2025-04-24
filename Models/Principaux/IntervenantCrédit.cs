using System.ComponentModel.DataAnnotations.Schema;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using Microsoft.Identity.Client;

namespace DCCR_SERVER.Models.Principaux
{
    public class IntervenantCrédit
    {
        public int id_intervenantcredit { get; set; }

        public string numero_contrat_credit {  get; set; }
        public DateOnly date_declaration {  get; set; }
        public int id_excel {  get; set; }
        public Crédit credit { get; set; }

        public string cle_intervenant {  get; set; } 
        public Intervenant intervenant { get; set; }

        public string niveau_responsabilite { get; set; }
        public NiveauResponsabilité niveau_resp { get; set; }
    }
}
