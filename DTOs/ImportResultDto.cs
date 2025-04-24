using System.Collections.Generic;
using DCCR_SERVER.Models.ValidationFichiers;

namespace DCCR_SERVER.DTOs
{
    public class ImportResultDto
    {
        public bool contientErreurs { get; set; }
        public List<Object> Erreurs { get; set; }
        public List<LoanPreviewDto> ApercuDonnees { get; set; }
        public int IdExcel { get; set; } // to track the import session/file
    }
}
