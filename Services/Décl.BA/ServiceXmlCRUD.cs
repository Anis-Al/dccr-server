using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using System.Xml;
using System.Text;
using DCCR_SERVER.Context;
using Microsoft.EntityFrameworkCore;
using DCCR_SERVER.Models.DTOs;

namespace DCCR_SERVER.Services.Décl.BA
{
    public class ServiceXmlCRUD
    {
        private readonly BddContext _context;

        public ServiceXmlCRUD(BddContext context)
        {
            _context = context;
        }

        public FichierXml genererDonneesFichiersXml(int idExcel)
        {
            var credits = _context.credits
                .AsNoTracking() 
                .Include(c => c.intervenantsCredit)
                    .ThenInclude(ic => ic.intervenant)
                .Include(c => c.garanties)
                .Include(c => c.lieu)
                .Where(c => c.id_excel == idExcel)
                .ToList();

            string correctionXml = genererXmlCorrection(credits);
            string suppressionXml = genererXmlSuppression(credits);

            var fichierXml = new FichierXml
            {
                nom_fichier_xml = $"crem_{DateTime.Now:yyyyMMdd_HHmmss}.xml",
                contenu_correction = correctionXml,
                contenu_supression = suppressionXml,
                id_fichier_excel = idExcel,
                id_utilisateur_generateur_xml = "anis2002"
            };

            return fichierXml;
        }

        private string genererXmlCorrection(List<Crédit> credits)
        {
            try
            {
                var creditsParDate = credits.GroupBy(c => c.date_declaration);
                var sb = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true
                };

                using (var writer = XmlWriter.Create(sb, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("crem");
                    writer.WriteAttributeString("c1", "1.0");

                    writer.WriteStartElement("c2");
                    writer.WriteAttributeString("c21", "021");
                    writer.WriteAttributeString("c22", "021");
                    writer.WriteAttributeString("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteAttributeString("c24", DateTime.Now.ToString("yyyyMMdd") + "999");
                    writer.WriteAttributeString("c25", "111");
                    writer.WriteEndElement();

                    writer.WriteStartElement("c3");

                    foreach (var dateGroup in creditsParDate)
                    {
                        writer.WriteStartElement("c31");
                        writer.WriteAttributeString("s1", dateGroup.Key.ToString("yyyy-MM-dd"));

                        var creditsByIntervenant = dateGroup
                            .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                            .GroupBy(x => x.Intervenant.cle);

                        foreach (var intervenantGroup in creditsByIntervenant)
                        {
                            writer.WriteStartElement("s2");

                            writer.WriteStartElement("d32");
                            var firstIntervenantRecord = intervenantGroup.First();
                            writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", firstIntervenantRecord.Intervenant.type_cle);
                            writer.WriteString(intervenantGroup.Key);
                            writer.WriteEndElement();

                            writer.WriteStartElement("s11");

                            foreach (var creditRecord in intervenantGroup)
                            {
                                var credit = creditRecord.Credit;
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

                                writer.WriteAttributeString("s124", credit.date_octroi.ToString("yyyy-MM-dd"));
                                writer.WriteAttributeString("s125", credit.date_expiration.ToString("yyyy-MM-dd"));
                                writer.WriteAttributeString("s102", creditRecord.IntervenantCredit.niveau_responsabilite);

                                if (!string.IsNullOrEmpty(creditRecord.Intervenant.rib))
                                    writer.WriteAttributeString("s113", creditRecord.Intervenant.rib);

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
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating correction XML: {ex.Message}", ex);
            }
        }

        private string genererXmlSuppression(List<Crédit> credits)
        {
            try
            {
                var creditsParDate = credits.GroupBy(c => c.date_declaration);
                var sb = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true
                };

                using (var writer = XmlWriter.Create(sb, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("crem");
                    writer.WriteAttributeString("c1", "1.0");

                    writer.WriteStartElement("c2");
                    writer.WriteAttributeString("c21", "021");
                    writer.WriteAttributeString("c22", "021");
                    writer.WriteAttributeString("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteAttributeString("c24", DateTime.Now.ToString("yyyyMMdd") + "999");
                    writer.WriteAttributeString("c25", "111");
                    writer.WriteEndElement();

                    writer.WriteStartElement("c3");

                    foreach (var dateUnique in creditsParDate)
                    {
                        writer.WriteStartElement("c31");
                        writer.WriteAttributeString("s1", dateUnique.Key.ToString("yyyy-MM-dd"));

                        var creditsByIntervenant = dateUnique
                            .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                            .GroupBy(x => x.Intervenant.cle);

                        foreach (var intervenantGroup in creditsByIntervenant)
                        {
                            writer.WriteStartElement("s2");

                            writer.WriteStartElement("d32");
                            var firstIntervenantRecord = intervenantGroup.First();
                            writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", firstIntervenantRecord.Intervenant.type_cle);
                            writer.WriteString(intervenantGroup.Key);
                            writer.WriteEndElement(); // d32

                            writer.WriteEndElement(); // s2
                        }
                        writer.WriteEndElement(); // c31
                    }
                    writer.WriteEndElement(); // c3
                    writer.WriteEndElement(); // crem
                    writer.WriteEndDocument();
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating suppression XML: {ex.Message}", ex);
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

        public async Task<(byte[] correctionContent, string correctionFileName, byte[] suppressionContent, string suppressionFileName)> getLesFichiersXml(int idXml)
        {
            var xmlFile = await getXmlParId(idXml);
            if (xmlFile == null)
            {
                return (null, null, null, null);
            }

            string correctionFileName = $"correction_{xmlFile.nom_fichier_xml}";
            byte[] correctionContent = convertirStringAuXml(xmlFile.contenu_correction);

            string suppressionFileName = $"suppression_{xmlFile.nom_fichier_xml}";
            byte[] suppressionContent = convertirStringAuXml(xmlFile.contenu_supression);

            return (correctionContent, correctionFileName, suppressionContent, suppressionFileName);
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
                NomFichierXml = fx.nom_fichier_xml,
                DateHeureGenerationXml = fx.date_heure_generation_xml,
                NomUtilisateurGenerateur = fx.generateurXml?.nom_complet ,
                NomFichierExcelSource = fx.fichier_excel?.nom_fichier_excel
            }).ToList();
        }

        //  public async Task<bool>archiverDonnees<>()       
        // {
        //     using var transaction = await _context.Database.BeginTransactionAsync();
        //     try
        //     {
               
        //     }
        //     catch (Exception ex)
        //     {
                
        //         await transaction.RollbackAsync();
        //         throw new Exception(ex,ex.Message);
        //     }
        // }
    
    }
}