using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.DTOs.Export;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DCCR_SERVER.Services.Excel
{
    public class ErreurExcelExportService
    {
        private readonly BddContext _context;

        public ErreurExcelExportService(BddContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportEtSuppressionErreursAsync(int idExcel)
        {
            var erreurs = await _context.erreurs_fichiers_excel
                .Where(e => e.id_excel == idExcel)
                .Select(e => new ErreurExcelSimpleDto
                {
                    ligne_excel = e.ligne_excel,
                    message_erreur = e.message_erreur
                })
                .OrderBy(e => e.ligne_excel)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Erreurs");
                worksheet.Cells[1, 1].Value = "Ligne";
                worksheet.Cells[1, 2].Value = "Message Erreur";

                for (int i = 0; i < erreurs.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = erreurs[i].ligne_excel;
                    worksheet.Cells[i + 2, 2].Value = erreurs[i].message_erreur;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var erreursToDelete = await _context.erreurs_fichiers_excel
                    .Where(e => e.id_excel == idExcel)
                    .ToListAsync();
                _context.erreurs_fichiers_excel.RemoveRange(erreursToDelete);

                var FichierASupprimer = await _context.fichiers_excel.FirstOrDefaultAsync(f => f.id_fichier_excel == idExcel);
                if (FichierASupprimer != null)
                {
                    _context.fichiers_excel.Remove(FichierASupprimer);
                }

                await _context.SaveChangesAsync();

                return package.GetAsByteArray();
            }
        }
    }
}