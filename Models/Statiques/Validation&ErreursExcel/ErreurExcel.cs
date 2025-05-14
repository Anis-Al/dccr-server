using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DCCR_SERVER.Models.Principaux;

namespace DCCR_SERVER.Models.ValidationFichiers
{
    public class ErreurExcel
    {

        public int id_erreur { get; set; }

        public int id_excel { get; set; }

        public int? id_regle { get; set; }
        public int ligne_excel { get; set; }
        public string? message_erreur { get; set; } 
        public FichierExcel excel_associe { get; set; }
        public RegleValidation regle_associe { get; set; }

    }
}
