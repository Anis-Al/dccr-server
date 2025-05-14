using System.Collections.Generic;
using DCCR_SERVER.Models.ValidationFichiers;

namespace DCCR_SERVER.DTOs.Excel
{
    public class ImportResultDto
    {
        public bool contientErreurs { get; set; }
        public List<object> Erreurs { get; set; }
        public List<ApercuCredit> ApercuDonnees { get; set; }
        public int IdExcel { get; set; }
        public string NomFichierExcel { get; internal set; }
    }
}
