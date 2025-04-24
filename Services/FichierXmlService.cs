using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DCCR_SERVER.Context;
using DCCR_SERVER.Models.Principaux;
using Microsoft.EntityFrameworkCore;

namespace DCCR_SERVER.Services
{
    public class FichierXmlService
    {
        private readonly BddContext _context;
        public FichierXmlService(BddContext context)
        {
            _context = context;
        }

        // Génère à la volée les deux XML (correction & suppression) pour un fichier Excel donné
        public async Task<(string CorrectionXml, string SuppressionXml)> GenerateXmlsForExcelAsync(int idFichierExcel)
        {
            var credits = await _context.credits.Where(c => c.id_excel == idFichierExcel).ToListAsync();
            if (!credits.Any())
                throw new InvalidOperationException("Aucun crédit trouvé pour ce fichier Excel.");
            string correction = GenerateXmlCorrection(credits);
            string suppression = GenerateXmlSuppression(credits);
            return (correction, suppression);
        }

        private string GenerateXmlCorrection(List<Crédit> credits)
        {
            // Remplacez ceci par la vraie sérialisation XML pour correction
            var xml = "<Corrections>" + string.Join("", credits.Select(c => $"<Credit numero='{c.numero_contrat_credit}' montant='{c.credit_accorde}' />")) + "</Corrections>";
            return xml;
        }

        private string GenerateXmlSuppression(List<Crédit> credits)
        {
            // Remplacez ceci par la vraie sérialisation XML pour suppression
            var xml = "<Suppressions>" + string.Join("", credits.Select(c => $"<Credit numero='{c.numero_contrat_credit}' />")) + "</Suppressions>";
            return xml;
        }
    }
}
