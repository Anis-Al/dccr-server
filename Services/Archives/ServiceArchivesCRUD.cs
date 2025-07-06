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

        public async Task<List<ArchiveCreditDto.ArchiveCreditDetailsDto>> GetDetailsCreditArchive(string num_contrat_credit, DateOnly date_declaration)
        {
            try
            {
                var creditsQuery = _contexte.archives_credits
                    .Where(c => (string.IsNullOrEmpty(num_contrat_credit) || c.numero_contrat_credit == num_contrat_credit) &&
                              (date_declaration == default || c.date_declaration == date_declaration))
                    .GroupJoin(_contexte.types_credit,
                        c => c.type_credit,
                        r => r.code,
                        (c, typeGroup) => new { credit = c, typeGroup })
                    .SelectMany(
                        x => x.typeGroup.DefaultIfEmpty(),
                        (x, typeCredit) => new { x.credit, typeCredit })
                    .GroupJoin(_contexte.activites_credit,
                        x => x.credit.activite_credit,
                        r => r.code,
                        (x, activiteGroup) => new { x.credit, x.typeCredit, activiteGroup })
                    .SelectMany(
                        x => x.activiteGroup.DefaultIfEmpty(),
                        (x, activite) => new { x.credit, x.typeCredit, activite })
                    .GroupJoin(_contexte.situations_credit,
                        x => x.credit.situation_credit,
                        r => r.code,
                        (x, situationGroup) => new { x.credit, x.typeCredit, x.activite, situationGroup })
                    .SelectMany(
                        x => x.situationGroup.DefaultIfEmpty(),
                        (x, situation) => new { x.credit, x.typeCredit, x.activite, situation })
                    ;
                var creditsData = await creditsQuery
                    .GroupJoin(_contexte.lieux,
                        x => x.credit.id_lieu,
                        l => l.id_lieu,
                        (x, lieuGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, lieuGroup })
                    .SelectMany(
                        x => x.lieuGroup.DefaultIfEmpty(),
                        (x, lieu) => new { x.credit, x.typeCredit, x.activite, x.situation, lieu })
                    .GroupJoin(_contexte.agences,
                        x => x.lieu != null ? x.lieu.code_agence : null,
                        a => a.code,
                        (x, agenceGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, agenceGroup })
                    .SelectMany(
                        x => x.agenceGroup.DefaultIfEmpty(),
                        (x, agence) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, agence })            
                    .GroupJoin(_contexte.wilayas,
                        x => x.lieu != null ? x.lieu.code_wilaya : null,
                        r => r.code,
                        (x, wilayaGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, wilayaGroup })
                    .SelectMany(
                        x => x.wilayaGroup.DefaultIfEmpty(),
                        (x, wilaya) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, wilaya })
                    .GroupJoin(_contexte.pays,
                        x => x.lieu != null ? x.lieu.code_pays : null,
                        r => r.code,
                        (x, paysGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, paysGroup })
                    .SelectMany(
                        x => x.paysGroup.DefaultIfEmpty(),
                        (x, pays) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, pays })
                    .GroupJoin(_contexte.monnaies,
                        x => x.credit.monnaie,
                        r => r.code,
                        (x, deviseGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, deviseGroup })
                    .SelectMany(
                        x => x.deviseGroup.DefaultIfEmpty(),
                        (x, devise) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, devise })
                    .GroupJoin(_contexte.durees_credit,
                        x => x.credit.duree_initiale,
                        r => r.code,
                        (x, dureeInitGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, agence = x.agence, x.wilaya, x.pays, x.devise, dureeInitGroup })
                    .SelectMany(
                        x => x.dureeInitGroup.DefaultIfEmpty(),
                        (x, dureeInit) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, x.devise, dureeInit })
                    .GroupJoin(_contexte.durees_credit,
                        x => x.credit.duree_restante,
                        r => r.code,
                        (x, dureeRestGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, x.devise, x.dureeInit, dureeRestGroup })
                    .SelectMany(
                        x => x.dureeRestGroup.DefaultIfEmpty(),
                        (x, dureeRest) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, x.devise, x.dureeInit, dureeRest })
                    .GroupJoin(_contexte.classes_retard,
                        x => x.credit.classe_retard,
                        r => r.code,
                        (x, classeRetardGroup) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, x.devise, x.dureeInit, x.dureeRest, classeRetardGroup })
                    .SelectMany(
                        x => x.classeRetardGroup.DefaultIfEmpty(),
                        (x, classeRetard) => new { x.credit, x.typeCredit, x.activite, x.situation, x.lieu, x.agence, x.wilaya, x.pays, x.devise, x.dureeInit, x.dureeRest, classeRetard })
                    .Select(x => new ArchiveCreditDto.ArchiveCreditDetailsDto
                    {
                        num_contrat_credit = x.credit.numero_contrat_credit,
                        date_declaration = x.credit.date_declaration,
                        type_credit = x.credit.type_credit,
                        libelle_type_credit = x.typeCredit.domaine,
                        est_plafond_accorde = x.credit.est_plafond_accorde,
                        id_plafond = x.credit.id_plafond,
                        code_activite = x.credit.activite_credit,
                        libelle_activite = x.activite.domaine,
                        situation = x.credit.situation_credit,
                        libelle_situation = x.situation.domaine,
                        motif = x.credit.motif,
                        code_agence = x.lieu != null && x.lieu.code_agence != null ? x.lieu.code_agence : (x.agence != null ? x.agence.code : null),
                        libelle_agence = x.agence.domaine,
                        code_wilaya = x.lieu != null ? x.lieu.code_wilaya : null,
                        libelle_wilaya = x.wilaya.domaine,
                        code_pays = x.lieu != null ? x.lieu.code_pays : null,
                        libelle_pays = x.pays.domaine,
                        credit_accorde = x.credit.credit_accorde,
                        monnaie = x.credit.monnaie,
                        libelle_monnaie = x.devise.domaine,
                        taux_interets = x.credit.taux,
                        cout_total_credit = x.credit.cout_total_credit,
                        solde_restant = x.credit.solde_restant,
                        mensualite = x.credit.mensualite,
                        duree_initiale = x.credit.duree_initiale,
                        libelle_duree_initiale = x.dureeInit.domaine,
                        duree_restante = x.credit.duree_restante,
                        libelle_duree_restante = x.dureeRest.domaine,
                        classe_retard = x.credit.classe_retard,
                        libelle_classe_retard = x.classeRetard != null ? x.classeRetard.domaine : null,
                        nombre_echeances_impayes = x.credit.nombre_echeances_impayes,
                        date_constatation_echeances_impayes = x.credit.date_constatation,
                        montant_capital_retard = x.credit.montant_capital_retard,
                        montant_interets_retard = x.credit.montant_interets_retard,
                        montant_interets_courus = x.credit.montant_interets_courus,
                        date_octroi = x.credit.date_octroi,
                        date_expiration = x.credit.date_expiration,
                        date_execution = x.credit.date_execution,
                        date_rejet = x.credit.date_rejet,
                        id_excel = x.credit.id_excel
                    })
                       
                    .ToListAsync();

                var creditNumbers = creditsData.Select(c => c.num_contrat_credit).Distinct().ToList();

                var intervenants = await (from ic in _contexte.archives_intervenants_credits
                    join i in _contexte.intervenants on ic.cle_intervenant equals i.cle
                    join r in _contexte.niveaux_responsabilite on ic.niveau_responsabilite equals r.code into rGroup
                    from r in rGroup.DefaultIfEmpty()
                    where creditNumbers.Contains(ic.numero_contrat_credit)
                    select new { ic, i, libelle = r.domaine })
                    .GroupBy(x => x.ic.numero_contrat_credit)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Select(x => new ArchiveCreditDto.ArchiveIntervenantDto
                        {
                            cle = x.i.cle,
                            type_cle = x.i.type_cle,
                            nif = x.i.nif,
                            rib = x.i.rib,
                            cli = x.i.cli,
                            niveau_responsabilite = x.ic.niveau_responsabilite,
                            libelle_niveau_responsabilite = x.libelle
                        }).ToList()
                    );

                var garanties = await (from g in _contexte.archives_garanties
                    join r in _contexte.types_garantie on g.type_garantie equals r.code into rGroup
                    from r in rGroup.DefaultIfEmpty()
                    where creditNumbers.Contains(g.numero_contrat_credit)
                    select new { g, libelle = r.domaine })
                    .GroupBy(x => x.g.numero_contrat_credit)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Select(x => new ArchiveCreditDto.ArchiveGarantieDto
                        {
                            type_garantie = x.g.type_garantie,
                            libelle_type_garantie = x.libelle,
                            montant_garantie = x.g.montant_garantie,
                        }).ToList()
                    );

                foreach (var credit in creditsData)
                {
                    if (intervenants.TryGetValue(credit.num_contrat_credit, out var inters))
                    {
                        credit.intervenants = inters;
                    }
                    if (garanties.TryGetValue(credit.num_contrat_credit, out var gars))
                    {
                        credit.garanties = gars;
                    }
                }

                return creditsData;
            }
            catch (Exception ex)
            {
                _journal.LogError(ex, "Erreur");
                throw;
            }
        }
    }
}
