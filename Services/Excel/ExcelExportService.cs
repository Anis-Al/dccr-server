using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DCCR_SERVER.Services.Excel
{
    public class ExcelExportService
    {
        public byte[] ExporterVersExcel<T>(IEnumerable<T> donnees, string nomFeuille = "export")
        {
            if (donnees == null)
                throw new ArgumentNullException(nameof(donnees));

            using (var classeur = new ExcelPackage())
            {
                var feuille = classeur.Workbook.Worksheets.Add(nomFeuille);
                var proprietes = typeof(T).GetProperties();

                for (int i = 0; i < proprietes.Length; i++)
                {
                    feuille.Cells[1, i + 1].Value = proprietes[i].Name;
                }

                int ligne = 2;
                foreach (var element in donnees)
                {
                    for (int colonne = 0; colonne < proprietes.Length; colonne++)
                    {
                        var valeur = proprietes[colonne].GetValue(element);
                        feuille.Cells[ligne, colonne + 1].Value = valeur ?? DBNull.Value;
                    }
                    ligne++;
                }

                if (proprietes.Length > 0 && ligne > 1)
                {
                    feuille.Cells[1, 1, ligne - 1, proprietes.Length].AutoFitColumns();
                }

                return classeur.GetAsByteArray();
            }
        }

        public async Task<byte[]> ExporterVersExcelAsync<T>(IEnumerable<T> donnees, string nomFeuille = "export")
        {
            return await Task.Run(() => ExporterVersExcel(donnees, nomFeuille));
        }
    }
}
