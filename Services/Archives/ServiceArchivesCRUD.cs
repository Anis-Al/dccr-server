using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs.Archives;
using DCCR_SERVER.Models.Principaux.Archives;
using Microsoft.EntityFrameworkCore;

namespace DCCR_SERVER.Services.Archives
{
    public class ServiceArchivesCRUD
    {
        private readonly BddContext _contexte;
        private readonly ILogger<ServiceArchivesCRUD> _journal;

        public ServiceArchivesCRUD(
            BddContext contexte,
            ILogger<ServiceArchivesCRUD> journal)
        {
            _contexte = contexte;
            _journal = journal;
        }

        public async Task<List<ArchiveDto>> getTousLesFichiersExcelArchives()
        {
            try
            {
                var requete = _contexte.archives_fichiers_excel
                    .AsNoTracking();

                var fichiers = await requete
                    .OrderByDescending(f => f.date_heure_integration_excel)
                    .Select(f => new ArchiveDto
                    {
                        IdFichierExcel = f.id_fichier_excel,
                        NomFichierExcel = f.nom_fichier_excel,
                        CheminFichierExcel = f.chemin_fichier_excel,
                        DateHeureIntegrationExcel = f.date_heure_integration_excel,
                        Integrateur = f.id_integrateur_excel
                    })
                    .ToListAsync();

                return fichiers;
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Erreur lors de la récupération des fichiers Excel archivés");
                throw;
            }
        }
        public async Task<List<ArchiveFichierXmlDto>> getTousLesFichiersXmlArchives()
        {
            try
            {
                var requete = _contexte.archives_fichiers_xml
                    .Include(fx => fx.fichier_excel)
                    .AsNoTracking();

                var fichiers = await requete
                    .OrderByDescending(f => f.date_heure_generation_xml)
                    .Select(f => new ArchiveFichierXmlDto
                    {
                        IdFichierXml = f.id_fichier_xml,
                        NomFichierCorrection = f.nom_fichier_correction,
                        NomFichierSuppression = f.nom_fichier_suppression,
                        DateHeureGenerationXml = f.date_heure_generation_xml,
                        NomUtilisateurGenerateur = f.id_utilisateur_generateur_xml,
                        NomFichierExcelSource = f.fichier_excel.nom_fichier_excel
                    })
                    .ToListAsync();

                return fichiers;
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Erreur lors de la récupération des fichiers XML archivés");
                throw;
            }
        }
        public async Task<List<ArchiveCreditDto.ArchiveCreditListeDto>> GetTousLesCreditsArchivesListe()
        {
            try
            {
                var requete = _contexte.archives_credits
                    .AsNoTracking();

                var credits = await requete
                    .OrderByDescending(c => c.date_declaration)
                    .ThenBy(c => c.numero_contrat_credit)
                    .Select(c => new ArchiveCreditDto.ArchiveCreditListeDto
                    {
                        num_contrat_credit = c.numero_contrat_credit,
                        date_declaration = c.date_declaration,
                        libelle_type_credit = c.type_credit,
                        libelle_activite = c.activite_credit,
                        libelle_situation = c.situation_credit,
                        id_excel = c.id_excel,
                    })
                    .ToListAsync();

                return credits;
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Erreur lors de la récupération de la liste des crédits archivés");
                throw;
            }
        }
    }
}
