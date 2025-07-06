using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.DTOs.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Services.Excel
{
    public class ServiceExcelCRUD
    {
        private readonly BddContext _contexte;
        public ServiceExcelCRUD(BddContext contexte)
        {
            _contexte = contexte;
        }

        public async Task<List<ExcelMetaDonneesDto>> getTousLesMetaDonneesDuExcel()
        {
            try
            {
                var requete = _contexte.fichiers_excel
                    .Where(fe => fe.statut_import == StatutImport.ImportConfirme || fe.statut_import == StatutImport.declarationGenere)
                    .AsNoTracking();

                var MetaDonnees = await requete
                    .OrderByDescending(fe => fe.id_fichier_excel)
                    .Select(emd => new ExcelMetaDonneesDto
                    {
                        id_fichier_excel=emd.id_fichier_excel,
                        nom_fichier_excel=emd.nom_fichier_excel,
                        chemin_fichier_excel=emd.chemin_fichier_excel,
                        date_heure_integration_excel=emd.date_heure_integration_excel,
                        integrateur = emd.integrateurExcel.nom_complet,
                    })
                    .ToListAsync();

                return MetaDonnees;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ExcelMetaDonneesDto>> getTousLesMetaDonneesDuExcelPourGenererDecls()
        {
            try
            {
                var requete = _contexte.fichiers_excel
                    .Where(fe => fe.statut_import == StatutImport.ImportConfirme)
                    .AsNoTracking();

                var MetaDonnees = await requete
                    .OrderByDescending(fe => fe.id_fichier_excel)
                    .Select(emd => new ExcelMetaDonneesDto
                    {
                        id_fichier_excel = emd.id_fichier_excel,
                        nom_fichier_excel = emd.nom_fichier_excel,
                        chemin_fichier_excel = emd.chemin_fichier_excel,
                        date_heure_integration_excel = emd.date_heure_integration_excel,
                        integrateur = emd.integrateurExcel.nom_complet,
                    })
                    .ToListAsync();

                return MetaDonnees;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> supprimerFichierExcel(int id_fichier_excel)
        {
            try
            {
                var fichierExcel = await _contexte.fichiers_excel
                    .FirstOrDefaultAsync(f => f.id_fichier_excel == id_fichier_excel);

                if (fichierExcel == null)
                    return false;

                _contexte.fichiers_excel.Remove(fichierExcel);
                await _contexte.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
