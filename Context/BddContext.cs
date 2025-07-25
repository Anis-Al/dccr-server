using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Principaux.Archives;
using DCCR_SERVER.Models.Statiques;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using DCCR_SERVER.Models.Utilisateurs_audit;
using DCCR_SERVER.Models.ValidationFichiers;
using Microsoft.EntityFrameworkCore;

namespace DCCR_SERVER.Context
{
    public class BddContext : DbContext
    {
        public BddContext(DbContextOptions<BddContext> options) : base(options) {}

        public DbSet<donnees_brutes> table_intermediaire_traitement { get; set; }
        public DbSet<Crédit> credits { get; set; }
        public DbSet<Intervenant> intervenants { get; set; }
        public DbSet<IntervenantCrédit> intervenants_credits { get; set; }
        public DbSet<Garantie> garanties { get; set; }
        public DbSet<FichierExcel> fichiers_excel { get; set; }
        public DbSet<FichierXml> fichiers_xml { get; set; }
        public DbSet<Lieu> lieux { get; set; }
        public DbSet<Audit> pistes_audit { get; set; }
        public DbSet<Utilisateur> utilisateurs { get; set; }
        public DbSet<TableauDeBord> tableau_de_bord { get; set; }
        public DbSet<MappingColonnes> mapping_colonnes { get; set; }
        public DbSet<ErreurExcel> erreurs_fichiers_excel { get; set; }
        public DbSet<RegleValidation> regles_validation { get; set; }
        public DbSet<Parametrage> parametrage { get; set; }
        public DbSet<ArchiveCrédit> archives_credits { get; set; }
        public DbSet<ArchiveFichierExcel> archives_fichiers_excel { get; set; }
        public DbSet<ArchiveFichierXml> archives_fichiers_xml { get; set; }
        public DbSet<ArchiveGarantie> archives_garanties { get; set; }
        public DbSet<ArchiveIntervenantCrédit> archives_intervenants_credits { get; set; }

        // Tables domaines
        public DbSet<ActivitéCrédit> activites_credit { get; set; }
        public DbSet<Monnaie> monnaies { get; set; }
        public DbSet<SituationCrédit> situations_credit { get; set; }
        public DbSet<DuréeCrédit> durees_credit { get; set; }
        public DbSet<TypeCrédit> types_credit { get; set; }
        public DbSet<TypeGarantie> types_garantie { get; set; }
        public DbSet<ClasseRetard> classes_retard { get; set; }
        public DbSet<Pays> pays { get; set; }
        public DbSet<Agence> agences { get; set; }
        public DbSet<NiveauResponsabilité> niveaux_responsabilite { get; set; }
        public DbSet<Wilaya> wilayas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crédit>().HasKey(c => new { c.numero_contrat_credit, c.date_declaration, c.id_excel });
            modelBuilder.Entity<Garantie>().HasKey(g => g.id_garantie);
            modelBuilder.Entity<Intervenant>().HasKey(i => i.cle);
            modelBuilder.Entity<IntervenantCrédit>().HasKey(ic => ic.id_intervenantcredit);
            modelBuilder.Entity<Lieu>().HasKey(l => l.id_lieu);
            modelBuilder.Entity<FichierXml>().HasKey(fx => fx.id_fichier_xml);
            modelBuilder.Entity<FichierExcel>().HasKey(fex => fex.id_fichier_excel);
            modelBuilder.Entity<ErreurExcel>().HasKey(ee => ee.id_erreur);
            modelBuilder.Entity<MappingColonnes>().HasKey(mc => mc.id_mapping);
            modelBuilder.Entity<Parametrage>().HasKey(pfx => pfx.parametre);
            modelBuilder.Entity<Utilisateur>().HasKey(u => u.matricule);
            modelBuilder.Entity<Audit>().HasKey(a => a.id_action);
            modelBuilder.Entity<RegleValidation>().HasKey(rv => rv.id_regle);
            modelBuilder.Entity<donnees_brutes>().HasKey(db => db.id);
            modelBuilder.Entity<TableauDeBord>().HasKey(tdb => tdb.id_kpi);

            modelBuilder.Entity<ActivitéCrédit>().HasKey(ac => ac.code);
            modelBuilder.Entity<Monnaie>().HasKey(m => m.code);
            modelBuilder.Entity<SituationCrédit>().HasKey(sc => sc.code);
            modelBuilder.Entity<DuréeCrédit>().HasKey(dc => dc.code);
            modelBuilder.Entity<TypeCrédit>().HasKey(tc => tc.code);
            modelBuilder.Entity<TypeGarantie>().HasKey(tg => tg.code);
            modelBuilder.Entity<ClasseRetard>().HasKey(cr => cr.code);
            modelBuilder.Entity<Pays>().HasKey(p => p.code);
            modelBuilder.Entity<Agence>().HasKey(a => a.code);
            modelBuilder.Entity<NiveauResponsabilité>().HasKey(nr => nr.code);
            modelBuilder.Entity<Wilaya>().HasKey(w => w.code);

            modelBuilder.Entity<Utilisateur>().HasIndex(u => u.matricule).IsUnique();
            modelBuilder.Entity<Intervenant>().HasIndex(i => i.cle).IsUnique();

            relations(modelBuilder);

            proprietes(modelBuilder);

            indexes(modelBuilder);

            archives(modelBuilder);
        }

        private void relations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FichierExcel>()
                .HasMany(fex => fex.erreurs)
                .WithOne(ee => ee.excel_associe)
                .HasForeignKey(ee => ee.id_excel)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FichierExcel>()
                .HasMany(fex => fex.credits)
                .WithOne(c => c.excel)
                .HasForeignKey(c => c.id_excel)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.lieu)
                .WithMany(l => l.credits)
                .HasForeignKey(c => c.id_lieu)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.devise)
                .WithMany(m => m.credits_monnaie)
                .HasForeignKey(c => c.monnaie)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.typecredit)
                .WithMany(tc => tc.credits_type_credit)
                .HasForeignKey(c => c.type_credit)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.situationcredit)
                .WithMany(sc => sc.credits_situation)
                .HasForeignKey(c => c.situation_credit)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.classeretard)
                .WithMany(cr => cr.credits_classe_retard)
                .HasForeignKey(c => c.classe_retard)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.activitecredit)
                .WithMany(ac => ac.credits_type_activite)
                .HasForeignKey(c => c.activite_credit)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.dureeinitiale)
                .WithMany(dc => dc.credits_duree_initiale)
                .HasForeignKey(c => c.duree_initiale)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasOne(c => c.dureerestante)
                .WithMany(dc => dc.credits_duree_restante)
                .HasForeignKey(c => c.duree_restante)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Crédit>()
                .HasMany(c => c.garanties)
                .WithOne(g => g.credit)
                .HasForeignKey(g => g.numero_contrat_credit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Crédit>()
                .HasMany(c => c.intervenantsCredit)
                .WithOne(ic => ic.credit)
                .HasForeignKey(ic => new { ic.numero_contrat_credit, ic.date_declaration, ic.id_excel })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Garantie>()
                .HasOne(g => g.typeGarantie)
                .WithMany(tg => tg.types_garantie)
                .HasForeignKey(g => g.type_garantie)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Garantie>()
                .HasOne(g => g.guarant)
                .WithMany(i => i.garanties_intervenant)
                .HasForeignKey(g => g.cle_interventant)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Garantie>()
                .HasOne(g => g.credit)
                .WithMany(c => c.garanties)
                .HasForeignKey(b => new { b.numero_contrat_credit, b.date_declaration, b.id_excel })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lieu>()
                .HasOne(l => l.agence)
                .WithMany(a => a.agences)
                .HasForeignKey(l => l.code_agence)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lieu>()
                .HasOne(l => l.wilaya)
                .WithMany(w => w.wilayas)
                .HasForeignKey(l => l.code_wilaya)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lieu>()
                .HasOne(l => l.pays)
                .WithMany(p => p.pays)
                .HasForeignKey(l => l.code_pays)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agence>()
                .HasOne(l => l.wilaya)
                .WithMany(p => p.agences)
                .HasForeignKey(l => l.wilaya_code)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Audit>()
                .HasOne(a => a.utilisateur_acteur)
                .WithMany(u => u.actions_de_cet_utilisateur)
                .HasForeignKey(a => a.matricule_utilisateur)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FichierXml>()
                .HasOne(fx => fx.generateurXml)
                .WithMany(u => u.fichiers_xml_générés)
                .HasForeignKey(fx => fx.id_utilisateur_generateur_xml)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FichierXml>()
                .HasOne(fx => fx.fichier_excel)
                .WithMany(fe => fe.fichiers_xml)
                .HasForeignKey(fx => fx.id_fichier_excel)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Intervenant>()
                .HasMany(i => i.intervenant_credits)
                .WithOne(ic => ic.intervenant)
                .HasForeignKey(ic => ic.cle_intervenant)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IntervenantCrédit>()
                .HasOne(ic => ic.niveau_resp)
                .WithMany(nr => nr.intervenants_niveau_resp)
                .HasForeignKey(ic => ic.niveau_responsabilite)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ErreurExcel>()
                .HasOne(ee => ee.regle_associe)
                .WithMany(rv => rv.erreurs)
                .HasForeignKey(ee => ee.id_regle)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FichierExcel>()
                .HasOne(fex => fex.integrateurExcel)
                .WithMany(u => u.fichiers_excel_integres)
                .HasForeignKey(fex => fex.id_integrateur_excel)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<donnees_brutes>()
                .HasOne(db => db.import_excel)
                .WithMany(f => f.donnees_brutes)
                .HasForeignKey(db => db.id_import_excel)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void proprietes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crédit>().Property(l => l.taux).HasColumnType("decimal(8,5)");
            modelBuilder.Entity<Crédit>().Property(l => l.credit_accorde).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.mensualite).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.cout_total_credit).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.montant_capital_retard).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.montant_interets_courus).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.montant_interets_retard).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.id_plafond).HasMaxLength(15);
            modelBuilder.Entity<Crédit>().Property(l => l.numero_contrat_credit).HasMaxLength(20);
            modelBuilder.Entity<Garantie>().Property(g => g.montant_garantie).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<Crédit>().Property(l => l.solde_restant).HasColumnType("decimal(18,0)");

            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.taux).HasColumnType("decimal(8,5)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.credit_accorde).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.mensualite).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.cout_total_credit).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.montant_capital_retard).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.montant_interets_courus).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.montant_interets_retard).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.id_plafond).HasMaxLength(15);
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.numero_contrat_credit).HasMaxLength(20);
            modelBuilder.Entity<ArchiveCrédit>().Property(l => l.solde_restant).HasColumnType("decimal(18,0)");
            modelBuilder.Entity<ArchiveGarantie>().Property(g => g.montant_garantie).HasColumnType("decimal(18,0)");

        }

        private void indexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crédit>().HasIndex(c => c.numero_contrat_credit);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.date_declaration);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.type_credit);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.monnaie);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.activite_credit);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.classe_retard);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.id_lieu);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.duree_initiale);
            modelBuilder.Entity<Crédit>().HasIndex(c => c.duree_restante);
            modelBuilder.Entity<Crédit>()
                .HasIndex(c => c.id_excel)
                .IncludeProperties(c => new {
                    c.numero_contrat_credit,
                    c.date_declaration,
                    c.type_credit,
                    c.activite_credit,
                    c.situation_credit
                });

            modelBuilder.Entity<Garantie>().HasIndex(g => g.id_garantie);
            modelBuilder.Entity<Garantie>().HasIndex(g => g.cle_interventant);
            modelBuilder.Entity<Garantie>().HasIndex(g => g.numero_contrat_credit);
            modelBuilder.Entity<Garantie>().HasIndex(g => g.date_declaration);
            modelBuilder.Entity<Garantie>().HasIndex(g => g.id_excel);

            modelBuilder.Entity<Intervenant>().HasIndex(i => i.cle);

            modelBuilder.Entity<IntervenantCrédit>().HasIndex(ic => ic.numero_contrat_credit);
            modelBuilder.Entity<IntervenantCrédit>().HasIndex(ic => ic.date_declaration);
            modelBuilder.Entity<IntervenantCrédit>().HasIndex(ic => ic.id_excel);
            modelBuilder.Entity<IntervenantCrédit>().HasIndex(ic => ic.cle_intervenant);
            modelBuilder.Entity<IntervenantCrédit>().HasIndex(ic => ic.niveau_responsabilite);

            modelBuilder.Entity<Lieu>().HasIndex(l => l.id_lieu);
            modelBuilder.Entity<Lieu>().HasIndex(l => l.code_agence);
            modelBuilder.Entity<Lieu>().HasIndex(l => l.code_wilaya);
            modelBuilder.Entity<Lieu>().HasIndex(l => l.code_pays);

            modelBuilder.Entity<FichierExcel>().HasIndex(fex => fex.id_fichier_excel);
            modelBuilder.Entity<FichierXml>().HasIndex(fx => fx.id_fichier_xml);
            modelBuilder.Entity<ErreurExcel>().HasIndex(ee => new { ee.id_excel, ee.ligne_excel });

            modelBuilder.Entity<RegleValidation>().HasIndex(rv => rv.nom_colonne);
            modelBuilder.Entity<donnees_brutes>().HasIndex(db => new { db.id_import_excel, db.est_valide, db.ligne_original }).IsClustered(false);
            modelBuilder.Entity<donnees_brutes>().HasIndex(x => new { x.numero_contrat, x.date_declaration });
            modelBuilder.Entity<donnees_brutes>().HasIndex(x => new { x.participant_cle, x.participant_type_cle });
            modelBuilder.Entity<Utilisateur>().HasIndex(u => u.matricule);

            modelBuilder.Entity<ActivitéCrédit>().HasIndex(ac => ac.code);
            modelBuilder.Entity<Monnaie>().HasIndex(m => m.code);
            modelBuilder.Entity<SituationCrédit>().HasIndex(sc => sc.code);
            modelBuilder.Entity<DuréeCrédit>().HasIndex(dc => dc.code);
            modelBuilder.Entity<TypeCrédit>().HasIndex(tc => tc.code);
            modelBuilder.Entity<TypeGarantie>().HasIndex(tg => tg.code);
            modelBuilder.Entity<ClasseRetard>().HasIndex(cr => cr.code);
            modelBuilder.Entity<Pays>().HasIndex(p => p.code);
            modelBuilder.Entity<Agence>().HasIndex(a => a.code);
            modelBuilder.Entity<NiveauResponsabilité>().HasIndex(nr => nr.code);
            modelBuilder.Entity<Wilaya>().HasIndex(w => w.code);
        }

        private void archives(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArchiveCrédit>().HasKey(c => new { c.numero_contrat_credit, c.date_declaration, c.id_excel });
            modelBuilder.Entity<ArchiveFichierExcel>().HasKey(fex => fex.id_fichier_excel);
            modelBuilder.Entity<ArchiveFichierXml>().HasKey(fx => fx.id_fichier_xml);
            modelBuilder.Entity<ArchiveGarantie>().HasKey(g => g.id_garantie);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasKey(ic => ic.id_intervenantcredit);

            modelBuilder.Entity<ArchiveCrédit>()
                .HasOne(ac => ac.excel)
                .WithMany(afe => afe.credits)
                .HasForeignKey(ac => ac.id_excel)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<ArchiveFichierXml>()
                .HasOne(afx => afx.fichier_excel)
                .WithMany(afe => afe.fichiers_xml)
                .HasForeignKey(afx => afx.id_fichier_excel)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<ArchiveGarantie>()
                .HasOne(ag => ag.credit)
                .WithMany(ac => ac.garanties)
                .HasForeignKey(ag => new { ag.numero_contrat_credit, ag.date_declaration, ag.id_excel })
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<ArchiveIntervenantCrédit>()
                .HasOne(aic => aic.credit)
                .WithMany(ac => ac.intervenantsCredit)
                .HasForeignKey(aic => new { aic.numero_contrat_credit, aic.date_declaration, aic.id_excel })
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<Intervenant>()
               .HasMany(i => i.intervenant_credits_archives)
               .WithOne(ic => ic.intervenant)
               .HasForeignKey(ic => ic.cle_intervenant)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArchiveCrédit>().HasIndex(ac => ac.numero_contrat_credit);
            modelBuilder.Entity<ArchiveCrédit>().HasIndex(ac => ac.date_declaration);
            modelBuilder.Entity<ArchiveCrédit>().HasIndex(ac => ac.id_excel);
            modelBuilder.Entity<ArchiveCrédit>().HasIndex(ac => ac.id_lieu);
            modelBuilder.Entity<ArchiveFichierExcel>().HasIndex(afe => afe.id_fichier_excel);
            modelBuilder.Entity<ArchiveFichierXml>().HasIndex(afx => afx.id_fichier_xml);
            modelBuilder.Entity<ArchiveFichierXml>().HasIndex(afx => afx.id_fichier_excel);
            modelBuilder.Entity<ArchiveGarantie>().HasIndex(ag => ag.id_garantie);
            modelBuilder.Entity<ArchiveGarantie>().HasIndex(ag => ag.cle_interventant);
            modelBuilder.Entity<ArchiveGarantie>().HasIndex(ag => ag.numero_contrat_credit);
            modelBuilder.Entity<ArchiveGarantie>().HasIndex(ag => ag.date_declaration);
            modelBuilder.Entity<ArchiveGarantie>().HasIndex(ag => ag.id_excel);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.id_intervenantcredit);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.numero_contrat_credit);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.date_declaration);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.id_excel);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.cle_intervenant);
            modelBuilder.Entity<ArchiveIntervenantCrédit>().HasIndex(aic => aic.niveau_responsabilite);
        }
    }
}