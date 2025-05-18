using DCCR_SERVER.Models.Principaux;
using DCCR_SERVER.Models.Statiques.TablesDomaines;
using System.Xml;
using System.Text;
using DCCR_SERVER.Context;
using Microsoft.EntityFrameworkCore;

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
            var creditsByDate = credits.GroupBy(c => c.date_declaration);

            var xml = new XmlDocument();

            var root = xml.CreateElement("crem");
            root.SetAttribute("c1", "1.0");
            xml.AppendChild(root);

            var c2 = xml.CreateElement("c2");
            c2.SetAttribute("c21", "021");
            c2.SetAttribute("c22", "021");
            c2.SetAttribute("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            c2.SetAttribute("c24", DateTime.Now.ToString("yyyyMMdd") + "999");
            c2.SetAttribute("c25", "111");
            root.AppendChild(c2);

            var c3 = xml.CreateElement("c3");
            root.AppendChild(c3);

            foreach (var dateGroup in creditsByDate)
            {
                var c31 = xml.CreateElement("c31");
                c31.SetAttribute("s1", dateGroup.Key.ToString("yyyy-MM-dd"));
                c3.AppendChild(c31);

                var creditsByIntervenant = dateGroup
                    .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                    .GroupBy(x => x.Intervenant.cle);

                foreach (var intervenantGroup in creditsByIntervenant)
                {
                    var s2 = xml.CreateElement("s2");
                    c31.AppendChild(s2);

                    var d32 = xml.CreateElement("d32");
                    var firstIntervenantRecord = intervenantGroup.First();
                    d32.InnerText = intervenantGroup.Key;
                    d32.SetAttribute("xsi:type", firstIntervenantRecord.Intervenant.type_cle);
                    s2.AppendChild(d32);

                    var s11 = xml.CreateElement("s11");
                    s2.AppendChild(s11);

                    foreach (var creditRecord in intervenantGroup)
                    {
                        var credit = creditRecord.Credit;
                        var s20 = xml.CreateElement("s20");

                        if (credit.id_plafond != null) s20.SetAttribute("s129", credit.id_plafond);
                        s20.SetAttribute("s128", credit.numero_contrat_credit);
                        s20.SetAttribute("s111", credit.monnaie);
                        s20.SetAttribute("s115", credit.activite_credit);
                        s20.SetAttribute("s107", credit.type_credit);
                        s20.SetAttribute("s103", credit.situation_credit);
                        if (credit.classe_retard != null) s20.SetAttribute("s104", credit.classe_retard);
                        s20.SetAttribute("s105", credit.duree_initiale);
                        s20.SetAttribute("s106", credit.duree_restante);
                        s20.SetAttribute("s117", credit.credit_accorde.ToString());
                        s20.SetAttribute("s101", credit.solde_restant.ToString());
                        s20.SetAttribute("s119", credit.cout_total_credit.ToString());
                        s20.SetAttribute("s110", credit.mensualite.ToString());
                        s20.SetAttribute("s118", credit.taux.ToString());

                        if (credit.date_constatation.HasValue) s20.SetAttribute("s120", credit.date_constatation.Value.ToString("yyyy-MM-dd"));
                        if (credit.nombre_echeances_impayes.HasValue)
                            s20.SetAttribute("s130", credit.nombre_echeances_impayes.Value.ToString());
                        if (credit.montant_interets_courus.HasValue)
                            s20.SetAttribute("s126", credit.montant_interets_courus.Value.ToString());
                        if (credit.montant_capital_retard.HasValue)
                            s20.SetAttribute("s121", credit.montant_capital_retard.Value.ToString());
                        if (credit.montant_interets_retard.HasValue)
                            s20.SetAttribute("s122", credit.montant_interets_retard.Value.ToString());
                        if (credit.date_rejet.HasValue)
                            s20.SetAttribute("s123", credit.date_rejet.Value.ToString("yyyy-MM-dd"));

                        s20.SetAttribute("s124", credit.date_octroi.ToString("yyyy-MM-dd"));
                        s20.SetAttribute("s125", credit.date_expiration.ToString("yyyy-MM-dd"));

                        s20.SetAttribute("s102", creditRecord.IntervenantCredit.niveau_responsabilite);

                        if (!string.IsNullOrEmpty(creditRecord.Intervenant.rib))
                            s20.SetAttribute("s113", creditRecord.Intervenant.rib);

                        if (credit.lieu != null)
                        {
                            s20.SetAttribute("s108", credit.lieu.code_pays);
                            s20.SetAttribute("s114", credit.lieu.code_agence);
                            s20.SetAttribute("s131", credit.lieu.code_wilaya);
                        }

                        if (credit.garanties != null && credit.garanties.Any())
                        {
                            var s109 = xml.CreateElement("s109");
                            s20.AppendChild(s109);

                            foreach (var garantie in credit.garanties)
                            {
                                var g = xml.CreateElement("g");
                                g.SetAttribute("g1", garantie.type_garantie);
                                g.SetAttribute("g2", garantie.montant_garantie.ToString());
                                s109.AppendChild(g);
                            }
                        }

                        s11.AppendChild(s20);
                    }
                }
            }

            return convertirXmlVersString(xml);
        }

        private string genererXmlSuppression(List<Crédit> credits)
        {
            var creditsParDate = credits.GroupBy(c => c.date_declaration);

            var xmlDoc = new XmlDocument();

            XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDecl);

            var root = xmlDoc.CreateElement("crem");
            root.SetAttribute("c1", "1.0");
            xmlDoc.AppendChild(root);

            var c2 = xmlDoc.CreateElement("c2");
            c2.SetAttribute("c21", "021");
            c2.SetAttribute("c22", "021");
            c2.SetAttribute("c23", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            c2.SetAttribute("c24", DateTime.Now.ToString("yyyyMMdd")+"999");
            c2.SetAttribute("c25", "111");
            root.AppendChild(c2);

            var c3 = xmlDoc.CreateElement("c3");
            root.AppendChild(c3);

            foreach (var dateUnique in creditsParDate)
            {
                var c31 = xmlDoc.CreateElement("c31");
                c31.SetAttribute("s1", dateUnique.Key.ToString("yyyy-MM-dd"));
                c3.AppendChild(c31);

                var creditsByIntervenant = dateUnique
                    .SelectMany(c => c.intervenantsCredit.Select(ic => new { Credit = c, Intervenant = ic.intervenant, IntervenantCredit = ic }))
                    .GroupBy(x => x.Intervenant.cle);

                foreach (var intervenantGroup in creditsByIntervenant)
                {
                    var s2 = xmlDoc.CreateElement("s2");
                    c31.AppendChild(s2);

                    var d32 = xmlDoc.CreateElement("d32");
                    var firstIntervenantRecord = intervenantGroup.First();
                    d32.InnerText = intervenantGroup.Key;
                    d32.SetAttribute("xsi:type", firstIntervenantRecord.Intervenant.type_cle);
                    s2.AppendChild(d32);

                }
            }

            return convertirXmlVersString(xmlDoc);
        }

        private string convertirXmlVersString(XmlDocument xmlDoc)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                xmlDoc.Save(writer);
            }

            return sb.ToString();
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
    }
}