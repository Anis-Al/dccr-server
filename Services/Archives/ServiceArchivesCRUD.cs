using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs.Archives;
using DCCR_SERVER.Models.Principaux.Archives;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
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
                    .Join(_contexte.utilisateurs,
                        f => f.id_integrateur_excel,
                        u => u.matricule,
                        (f, u) => new { Fichier = f, Utilisateur = u })
                    .Select(x => new ArchiveDto
                    {
                        IdFichierExcel = x.Fichier.id_fichier_excel,
                        NomFichierExcel = x.Fichier.nom_fichier_excel,
                        CheminFichierExcel = x.Fichier.chemin_fichier_excel,
                        DateHeureIntegrationExcel = x.Fichier.date_heure_integration_excel,
                        Integrateur = x.Utilisateur.nom_complet
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
                    .Join(_contexte.utilisateurs,
                        f => f.id_utilisateur_generateur_xml,
                        u => u.matricule,
                        (f, u) => new { Fichier = f, Utilisateur = u })
                    .Select(x => new ArchiveFichierXmlDto
                    {
                        IdFichierXml = x.Fichier.id_fichier_xml,
                        NomFichierCorrection = x.Fichier.nom_fichier_correction,
                        NomFichierSuppression = x.Fichier.nom_fichier_suppression,
                        DateHeureGenerationXml = x.Fichier.date_heure_generation_xml,
                        NomUtilisateurGenerateur = x.Utilisateur.nom_complet,
                        NomFichierExcelSource = x.Fichier.fichier_excel.nom_fichier_excel
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
                    .Join(_contexte.Set<TypeCrédit>(),
                        c => c.type_credit,
                        t => t.code,
                        (c, t) => new { Credit = c, TypeLibelle = t.domaine })
                    .Join(_contexte.Set<ActivitéCrédit>(),
                        ct => ct.Credit.activite_credit,
                        a => a.code,
                        (ct, a) => new { ct.Credit, ct.TypeLibelle, ActiviteLibelle = a.domaine })
                    .Join(_contexte.Set<SituationCrédit>(),
                        cta => cta.Credit.situation_credit,
                        s => s.code,
                        (cta, s) => new { cta.Credit, cta.TypeLibelle, cta.ActiviteLibelle, SituationLibelle = s.domaine })
                    .Select(x => new ArchiveCreditDto.ArchiveCreditListeDto
                    {
                        num_contrat_credit = x.Credit.numero_contrat_credit,
                        date_declaration = x.Credit.date_declaration,
                        libelle_type_credit = x.TypeLibelle,
                        libelle_activite = x.ActiviteLibelle,
                        libelle_situation = x.SituationLibelle,
                        id_excel = x.Credit.id_excel,
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
