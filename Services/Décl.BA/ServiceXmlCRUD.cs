using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using System.Xml;
using System.Text;
using DCCR_SERVER.Context;
using Microsoft.EntityFrameworkCore;
using DCCR_SERVER.Models.DTOs;
using DocumentFormat.OpenXml.InkML;
using Azure;
using DCCR_SERVER.Models.Principaux.Archives;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Services.Décl.BA
{
    public class ServiceXmlCRUD
    {
        private readonly BddContext _context;

        public ServiceXmlCRUD(BddContext context)
        {
            _context = context;
        }
        Stream outputStream;
        public async Task<bool> MigrerVersArchivesAsync(int idExcel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var fichierExcel = await _context.fichiers_excel
                    .FirstOrDefaultAsync(fe => fe.id_fichier_excel == idExcel);

                if (fichierExcel == null)
                {
                    throw new Exception("Fichier Excel non trouvé");
                }


                var dejaArchive = await _context.archives_fichiers_excel
                    .AnyAsync(afe => afe.id_fichier_excel == idExcel);

                if (dejaArchive)
                {
                    throw new Exception("Ce fichier a déjà été archivé");
                }


                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_fichiers_excel ON");

                try
                {
                    var archiveFichierExcel = new ArchiveFichierExcel
                    {
                        id_fichier_excel = fichierExcel.id_fichier_excel,
                        nom_fichier_excel = fichierExcel.nom_fichier_excel,
                        chemin_fichier_excel = fichierExcel.chemin_fichier_excel,
                        id_integrateur_excel = fichierExcel.id_integrateur_excel,
                        date_heure_integration_excel = fichierExcel.date_heure_integration_excel
                    };

                    _context.archives_fichiers_excel.Add(archiveFichierExcel);
                    await _context.SaveChangesAsync();
                }
                finally
                {
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_fichiers_excel OFF");
                }


                var credits = await _context.credits
                    .Include(c => c.garanties)
                    .Include(c => c.intervenantsCredit)
                    .Where(c => c.id_excel == idExcel)
                    .ToListAsync();

                foreach (var credit in credits)
                {
                    var archiveCredit = new ArchiveCrédit
                    {
                        numero_contrat_credit = credit.numero_contrat_credit,
                        date_declaration = credit.date_declaration,
                        id_excel = credit.id_excel,
                        est_plafond_accorde = credit.est_plafond_accorde,
                        situation_credit = credit.situation_credit,
                        date_octroi = credit.date_octroi,
                        date_rejet = credit.date_rejet,
                        date_expiration = credit.date_expiration,
                        date_execution = credit.date_execution,
                        duree_initiale = credit.duree_initiale,
                        duree_restante = credit.duree_restante,
                        id_lieu = credit.id_lieu,
                        type_credit = credit.type_credit,
                        activite_credit = credit.activite_credit,
                        monnaie = credit.monnaie,
                        credit_accorde = credit.credit_accorde ?? 0,
                        id_plafond = credit.id_plafond,
                        taux = credit.taux ?? 0,
                        mensualite = credit.mensualite ?? 0,
                        cout_total_credit = credit.cout_total_credit ?? 0,
                        solde_restant = credit.solde_restant ?? 0,
                        classe_retard = credit.classe_retard,
                        date_constatation = credit.date_constatation,
                        nombre_echeances_impayes = credit.nombre_echeances_impayes,
                        montant_interets_courus = credit.montant_interets_courus,
                        montant_interets_retard = credit.montant_interets_retard,
                        montant_capital_retard = credit.montant_capital_retard,
                        motif = credit.motif
                    };

                    _context.archives_credits.Add(archiveCredit);
                    await _context.SaveChangesAsync(); 

                    foreach (var garantie in credit.garanties)
                    {
                        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_garanties ON");
                        try
                        {
                            var archiveGarantie = new ArchiveGarantie
                            {
                                id_garantie = garantie.id_garantie,
                                cle_interventant = garantie.cle_interventant,
                                numero_contrat_credit = garantie.numero_contrat_credit,
                                date_declaration = garantie.date_declaration,
                                id_excel = garantie.id_excel,
                                type_garantie = garantie.type_garantie,
                                montant_garantie = garantie.montant_garantie
                            };
                            _context.Add(archiveGarantie);
                            await _context.SaveChangesAsync();
                        }
                        finally
                        {
                            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_garanties OFF");
                        }
                    }

                    foreach (var intervenantCredit in credit.intervenantsCredit)
                    {
                        var archiveIntervenantCredit = new ArchiveIntervenantCrédit
                        {
                            numero_contrat_credit = credit.numero_contrat_credit,
                            date_declaration = credit.date_declaration,
                            id_excel = credit.id_excel,
                            cle_intervenant = intervenantCredit.cle_intervenant,
                            niveau_responsabilite = intervenantCredit.niveau_responsabilite,
                        };
                        _context.Add(archiveIntervenantCredit);
                    }
                    await _context.SaveChangesAsync();
                }

                _context.garanties.RemoveRange(_context.garanties.Where(g => g.id_excel == idExcel));
                _context.intervenants_credits.RemoveRange(
                    _context.intervenants_credits.Where(ic => ic.id_excel == idExcel));
                _context.credits.RemoveRange(credits);
                var fichierXml = await _context.fichiers_xml
                    .FirstOrDefaultAsync(fx => fx.id_fichier_excel == idExcel);

                if (fichierXml != null)
                {
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_fichiers_xml ON");
                    try
                    {
                        var archiveFichierXml = new ArchiveFichierXml
                        {
                            id_fichier_xml = fichierXml.id_fichier_xml,
                            nom_fichier_correction = fichierXml.nom_fichier_correction,
                            nom_fichier_suppression = fichierXml.nom_fichier_suppression,
                            contenu_correction = fichierXml.contenu_correction,
                            contenu_suppression = fichierXml.contenu_suppression,
                            id_utilisateur_generateur_xml = fichierXml.id_utilisateur_generateur_xml,
                            date_heure_generation_xml = fichierXml.date_heure_generation_xml,
                            id_fichier_excel = fichierXml.id_fichier_excel
                        };
                        _context.archives_fichiers_xml.Add(archiveFichierXml);
                        await _context.SaveChangesAsync();
                    }
                    finally
                    {
                        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.archives_fichiers_xml OFF");
                    }

                    _context.fichiers_xml.Remove(fichierXml);
                }


                _context.fichiers_excel.Remove(fichierExcel);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"rollback: {rollbackEx.Message}");
                }
                throw new Exception($"Erreur : {ex.Message}", ex);
            }
        }

        public async Task<FichierXml> genererDonneesFichiersXmlAsync(int idExcel)
        {
            var credits = _context.credits
                .AsNoTracking() 
                .Include(c => c.intervenantsCredit)
                    .ThenInclude(ic => ic.intervenant)
                .Include(c => c.garanties)
                .Include(c => c.lieu)
                .Where(c => c.id_excel == idExcel)
                .ToList();

            var parametreSequence = _context.parametrage.FirstOrDefault(p => p.parametre == "sequence_dccr_actuelle");
            if (parametreSequence == null)
            {
                throw new Exception("Le paramètre 'sequence_dccr_actuelle' pas trouvé dans la table parametrage");
            }

            int sequenceActuelle = parametreSequence.valeur;
            
            int suppressionSequence = sequenceActuelle;
            int correctionSequence = sequenceActuelle + 1;
            int sequenceProchaine = sequenceActuelle + 2;
            
            if (correctionSequence > 999) correctionSequence = 1;
            
            if (sequenceProchaine > 999) sequenceProchaine = 1;
            
            parametreSequence.valeur = sequenceProchaine;
            _context.parametrage.Update(parametreSequence);
            await _context.SaveChangesAsync();
            
            string sequenceSuppression = suppressionSequence.ToString().PadLeft(3, '0');
            string sequenceCorrection = correctionSequence.ToString().PadLeft(3, '0');

            string correctionXml = genererXmlCorrection(credits, sequenceCorrection);
            string suppressionXml = genererXmlSuppression(credits, sequenceSuppression);

            var fichierXml = new FichierXml
            {
                nom_fichier_suppression = $"CREM.021.{DateTime.Now:yyyyMMdd}{sequenceSuppression}.DCCR.{DateTime.Now:yyyyMMdd.hhMMss}.xml",
                nom_fichier_correction = $"CREM.021.{DateTime.Now:yyyyMMdd}{sequenceCorrection}.DCCR.{DateTime.Now:yyyyMMdd.hhMMss}.xml",
                contenu_correction = correctionXml,
                contenu_suppression = suppressionXml,
                id_fichier_excel = idExcel,
                id_utilisateur_generateur_xml = "anis2002"
            };

            var fichierExcel = await _context.fichiers_excel
                .FirstOrDefaultAsync(f => f.id_fichier_excel == idExcel);
            if (fichierExcel != null)
            {
                fichierExcel.statut_import = StatutImport.declarationGenere;
                await _context.SaveChangesAsync();
            }

            return fichierXml;
        }

        private string genererXmlCorrection(List<Crédit> credits, string sequenceCorrection)
        {
            try
            {
                var creditsParDate = credits.GroupBy(c => c.date_declaration);
                using var ms = new MemoryStream();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Encoding = new UTF8Encoding(false),
                    OmitXmlDeclaration = false
                };

                using (var writer = XmlWriter.Create(ms, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("crem");
                    writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("c1", "1.0");

                    writer.WriteStartElement("c2");
                    writer.WriteAttributeString("c21", "021");
                    writer.WriteAttributeString("c22", "021");
                    writer.WriteAttributeString("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteAttributeString("c24", DateTime.Now.ToString("yyyyMMdd") + sequenceCorrection);
                    writer.WriteAttributeString("c25", "111");
                    writer.WriteEndElement();

                    writer.WriteStartElement("c3");

                    foreach (var date in creditsParDate)
                    {
                        writer.WriteStartElement("c31");
                        writer.WriteAttributeString("s1", date.Key.ToString("yyyy-MM-dd"));

                        var creditsParPersonne = date
                            .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                            .GroupBy(x => x.Intervenant.cle);

                        foreach (var personne in creditsParPersonne)
                        {
                            writer.WriteStartElement("s2");

                            writer.WriteStartElement("d32");
                            var instance_personne_unique = personne.First();
                            writer.WriteAttributeString("xsi", "type", null, instance_personne_unique.Intervenant.type_cle);
                            writer.WriteString(personne.Key.Trim());
                            writer.WriteEndElement();

                            writer.WriteStartElement("s11");

                            foreach (var creditsCettePersonne in personne)
                            {
                                var credit = creditsCettePersonne.Credit;
                                writer.WriteStartElement("s20");

                                if (credit.id_plafond != null) writer.WriteAttributeString("s129", credit.id_plafond);
                                writer.WriteAttributeString("s128", credit.numero_contrat_credit);
                                writer.WriteAttributeString("s111", credit.monnaie);
                                writer.WriteAttributeString("s115", credit.activite_credit);
                                writer.WriteAttributeString("s107", credit.type_credit);
                                writer.WriteAttributeString("s103", credit.situation_credit);
                                if (credit.classe_retard != null) writer.WriteAttributeString("s104", credit.classe_retard);
                                writer.WriteAttributeString("s105", credit.duree_initiale);
                                writer.WriteAttributeString("s106", credit.duree_restante);
                                writer.WriteAttributeString("s117", credit.credit_accorde.ToString());
                                writer.WriteAttributeString("s101", credit.solde_restant.ToString());
                                writer.WriteAttributeString("s119", credit.cout_total_credit.ToString());
                                writer.WriteAttributeString("s110", credit.mensualite.ToString());
                                writer.WriteAttributeString("s118", credit.taux.ToString());

                                if (credit.date_constatation.HasValue)
                                    writer.WriteAttributeString("s120", credit.date_constatation.Value.ToString("yyyy-MM-dd"));
                                if (credit.nombre_echeances_impayes.HasValue)
                                    writer.WriteAttributeString("s130", credit.nombre_echeances_impayes.Value.ToString());
                                if (credit.montant_interets_courus.HasValue)
                                    writer.WriteAttributeString("s126", credit.montant_interets_courus.Value.ToString());
                                if (credit.montant_capital_retard.HasValue)
                                    writer.WriteAttributeString("s121", credit.montant_capital_retard.Value.ToString());
                                if (credit.montant_interets_retard.HasValue)
                                    writer.WriteAttributeString("s122", credit.montant_interets_retard.Value.ToString());
                                if (credit.date_rejet.HasValue)
                                    writer.WriteAttributeString("s123", credit.date_rejet.Value.ToString("yyyy-MM-dd"));

                                writer.WriteAttributeString("s124", credit.date_octroi?.ToString("yyyy-MM-dd"));
                                writer.WriteAttributeString("s125", credit.date_expiration?.ToString("yyyy-MM-dd"));
                                writer.WriteAttributeString("s102", creditsCettePersonne.IntervenantCredit.niveau_responsabilite);

                                if (!string.IsNullOrEmpty(creditsCettePersonne.Intervenant.rib))
                                    writer.WriteAttributeString("s113", creditsCettePersonne.Intervenant.rib);

                                if (credit.lieu != null)
                                {
                                    writer.WriteAttributeString("s108", credit.lieu.code_pays);
                                    writer.WriteAttributeString("s114", credit.lieu.code_agence);
                                    writer.WriteAttributeString("s131", credit.lieu.code_wilaya);
                                }

                                if (credit.garanties != null && credit.garanties.Any())
                                {
                                    writer.WriteStartElement("s109");
                                    foreach (var garantie in credit.garanties)
                                    {
                                        writer.WriteStartElement("g");
                                        writer.WriteAttributeString("g1", garantie.type_garantie);
                                        writer.WriteAttributeString("g2", garantie.montant_garantie.ToString());
                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement(); // s11
                            writer.WriteEndElement(); // s2
                        }
                        writer.WriteEndElement(); // c31
                    }
                    writer.WriteEndElement(); // c3
                    writer.WriteEndElement(); // crem
                    writer.WriteEndDocument();
                }
                ms.Position = 0;
                using var reader = new StreamReader(ms, new UTF8Encoding(false));
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string genererXmlSuppression(List<Crédit> credits, string sequenceSuppression)
        {
            try
            {
                var creditsParDate = credits.GroupBy(c => c.date_declaration);
                using var ms = new MemoryStream();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Encoding = new UTF8Encoding(false),
                    OmitXmlDeclaration = false
                };

                using (var writer = XmlWriter.Create(ms, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("crem");
                    writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("c1", "1.0");

                    writer.WriteStartElement("c2");
                    writer.WriteAttributeString("c21", "021");
                    writer.WriteAttributeString("c22", "021");
                    writer.WriteAttributeString("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteAttributeString("c24", DateTime.Now.ToString("yyyyMMdd") + sequenceSuppression);
                    writer.WriteAttributeString("c25", "111");
                    writer.WriteEndElement();

                    writer.WriteStartElement("c3");

                    foreach (var date in creditsParDate)
                    {
                        writer.WriteStartElement("c31");
                        writer.WriteAttributeString("s1", date.Key.ToString("yyyy-MM-dd"));

                        var creditsParPersonne = date
                            .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                            .GroupBy(x => x.Intervenant.cle);

                        foreach (var personne in creditsParPersonne)
                        {
                            writer.WriteStartElement("s2");

                            writer.WriteStartElement("d32");
                            var instanceUniquePersonne = personne.First();
                            writer.WriteAttributeString("xsi", "type", null, instanceUniquePersonne.Intervenant.type_cle);
                            writer.WriteString(personne.Key.Trim());
                            writer.WriteEndElement(); // d32

                            writer.WriteEndElement(); // s2
                        }
                        writer.WriteEndElement(); // c31
                    }
                    writer.WriteEndElement(); // c3
                    writer.WriteEndElement(); // crem
                    writer.WriteEndDocument();
                }
                ms.Position = 0;
                using var reader = new StreamReader(ms, new UTF8Encoding(false));
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //------------------------------------------------

        public async Task<FichierXml> getXmlParId(int idXml)
        {
            return await _context.fichiers_xml
                .FirstOrDefaultAsync(fx => fx.id_fichier_xml == idXml);
        }

        public byte[] convertirStringAuXml(string xmlContent)
        {
            return Encoding.UTF8.GetBytes(xmlContent);
        }

        public async Task<(byte[] fichierCorrection, string nomFichierCorrection, byte[] fichierSuppression, string nomFichierSuppression)> envoyerLesDonneesDeDeclarationPourTelecharger(int idXml)
        {
            var instanceDeclaration = await getXmlParId(idXml);
            if (instanceDeclaration == null)
            {
                return (null, null, null, null);
            }

            string nomFichierCorrection = $"{instanceDeclaration.nom_fichier_correction}";
            byte[] fichierCorrection = convertirStringAuXml(instanceDeclaration.contenu_correction);

            string nomFichierSuppression = $"{instanceDeclaration.nom_fichier_suppression}";
            byte[] fichierSuppression = convertirStringAuXml(instanceDeclaration.contenu_suppression);

            return (fichierCorrection, nomFichierCorrection, fichierSuppression, nomFichierSuppression);
        }
        public async Task<List<XmlDto>> getTousLesFichiersXml()
        {
            var fichiers = await _context.fichiers_xml
                .Include(fx => fx.fichier_excel)
                .Include(fx => fx.generateurXml)
                .AsNoTracking()
                .ToListAsync();
                
            return fichiers.Select(fx => new XmlDto
            {
                IdFichierXml = fx.id_fichier_xml,
                NomFichierCorrection = fx.nom_fichier_correction,
                NomFichierSuppression = fx.nom_fichier_suppression,
                DateHeureGenerationXml = fx.date_heure_generation_xml,
                NomUtilisateurGenerateur = fx.generateurXml?.nom_complet,
                IdFichierExcelSource = fx.id_fichier_excel,
                NomFichierExcelSource = fx.fichier_excel?.nom_fichier_excel
            }).ToList();
        }
        public async Task<bool> supprimerDeclaration(int id_fichier_xml)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var declarationba = await _context.fichiers_xml
                    .Include(f => f.fichier_excel)
                    .FirstOrDefaultAsync(f => f.id_fichier_xml == id_fichier_xml);
                if (declarationba == null)
                    return false;
                if (declarationba.fichier_excel != null)
                {
                    declarationba.fichier_excel.statut_import = StatutImport.ImportConfirme;
                }
                _context.fichiers_xml.Remove(declarationba);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}