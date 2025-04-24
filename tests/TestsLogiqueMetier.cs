    using Xunit;
using DCCR_SERVER.Services.Excel;
using DCCR_SERVER.DTOs;
using DCCR_SERVER.Models.Principaux;
using System.Collections.Generic;

namespace DCCR_SERVER.Tests
{
    public class TestsLogiqueMetier
    {
        [Fact]
        public void ApercuPret_Retourne_Apercu_Correct()
        {
            var serviceIntegration = new ServiceIntegration(null, null, null);
            var donneesBrutes = new List<donnees_brutes>
            {
                new donnees_brutes
                {
                    id = 1,
                    id_import_excel = 10,
                    id_session_import = Guid.NewGuid(),
                    ligne_original = 1,
                    numero_contrat = "C1",
                    date_declaration = "2025-04-23",
                    situation_credit = "Situation1",
                    date_octroi = "2025-01-01",
                    date_rejet = "",
                    date_expiration = "2026-01-01",
                    date_execution = "2025-01-05",
                    duree_initiale = "12",
                    duree_restante = "10",
                    type_credit = "Type1",
                    activite_credit = "Commerce",
                    monnaie = "DZD",
                    credit_accorde = "100000",
                    id_plafond = "PLF1",
                    taux = "5.5",
                    mensualite = "8500",
                    cout_total_credit = "102000",
                    solde_restant = "90000",
                    classe_retard = "A",
                    date_constatation = "2025-06-01",
                    nombre_echeances_impayes = "0",
                    montant_interets_courus = "500",
                    montant_interets_retard = "0",
                    montant_capital_retard = "0",
                    motif = "",
                    participant_cle = "P1",
                    participant_type_cle = "TypeA",
                    participant_nif = "NIF1",
                    participant_cli = "CLI1",
                    participant_rib = "RIB1",
                    role_niveau_responsabilite = "Responsable",
                    garantie_type_garantie = "G1",
                    garantie_montant_garantie = "1000",
                    code_agence = "AG01",
                    code_wilaya = "16",
                    code_pays = "DZ",
                    est_valide = true,
                    messages_validation = "OK",
                    import_excel = null
                },
                new donnees_brutes
                {
                    id = 2,
                    id_import_excel = 10,
                    id_session_import = Guid.NewGuid(),
                    ligne_original = 2,
                    numero_contrat = "C1",
                    date_declaration = "2025-04-23",
                    situation_credit = "Situation2",
                    date_octroi = "2025-01-02",
                    date_rejet = "",
                    date_expiration = "2026-01-02",
                    date_execution = "2025-01-06",
                    duree_initiale = "12",
                    duree_restante = "11",
                    type_credit = "Type2",
                    activite_credit = "Service",
                    monnaie = "DZD",
                    credit_accorde = "200000",
                    id_plafond = "PLF2",
                    taux = "6.0",
                    mensualite = "17000",
                    cout_total_credit = "204000",
                    solde_restant = "180000",
                    classe_retard = "B",
                    date_constatation = "2025-06-02",
                    nombre_echeances_impayes = "1",
                    montant_interets_courus = "1000",
                    montant_interets_retard = "100",
                    montant_capital_retard = "1000",
                    motif = "",
                    participant_cle = "P2",
                    participant_type_cle = "TypeB",
                    participant_nif = "NIF2",
                    participant_cli = "CLI2",
                    participant_rib = "RIB2",
                    role_niveau_responsabilite = "Collaborateur",
                    garantie_type_garantie = "G2",
                    garantie_montant_garantie = "2000",
                    code_agence = "AG01",
                    code_wilaya = "16",
                    code_pays = "DZ",
                    est_valide = true,
                    messages_validation = "OK",
                    import_excel = null
                },
                new donnees_brutes
                {
                    id = 3,
                    id_import_excel = 10,
                    id_session_import = Guid.NewGuid(),
                    ligne_original = 3,
                    numero_contrat = "C2",
                    date_declaration = "2025-04-23",
                    situation_credit = "Situation3",
                    date_octroi = "2025-01-03",
                    date_rejet = "",
                    date_expiration = "2026-01-03",
                    date_execution = "2025-01-07",
                    duree_initiale = "24",
                    duree_restante = "22",
                    type_credit = "Type3",
                    activite_credit = "Industrie",
                    monnaie = "DZD",
                    credit_accorde = "300000",
                    id_plafond = "PLF3",
                    taux = "7.0",
                    mensualite = "25000",
                    cout_total_credit = "306000",
                    solde_restant = "270000",
                    classe_retard = "C",
                    date_constatation = "2025-06-03",
                    nombre_echeances_impayes = "2",
                    montant_interets_courus = "1500",
                    montant_interets_retard = "200",
                    montant_capital_retard = "2000",
                    motif = "",
                    participant_cle = "P3",
                    participant_type_cle = "TypeC",
                    participant_nif = "NIF3",
                    participant_cli = "CLI3",
                    participant_rib = "RIB3",
                    role_niveau_responsabilite = "Observateur",
                    garantie_type_garantie = null,
                    garantie_montant_garantie = null,
                    code_agence = "AG01",
                    code_wilaya = "16",
                    code_pays = "DZ",
                    est_valide = true,
                    messages_validation = "OK",
                    import_excel = null
                }
            };

            var resultat = serviceIntegration.BuildLoanPreview(donneesBrutes);

            // Affichage du résultat pour inspection
            foreach (var contrat in resultat)
            {
                Console.WriteLine($"Contrat: {contrat.NumeroContrat}, Date: {contrat.DateDeclaration}");
                Console.WriteLine($"  Participants: {contrat.Participants.Count}");
                foreach (var p in contrat.Participants)
                {
                    Console.WriteLine($"    - {p.ParticipantCle}, Type: {p.ParticipantType}, NIF: {p.ParticipantNif}, CLI: {p.ParticipantCli}, RIB: {p.ParticipantRib}");
                }
                Console.WriteLine($"  Garanties: {contrat.Garanties.Count}");
                foreach (var g in contrat.Garanties)
                {
                    Console.WriteLine($"    - Type: {g.TypeGarantie}, Montant: {g.MontantGarantie}");
                }
            }

            Assert.Equal(2, resultat.Count);
            var contrat1 = resultat.Find(l => l.NumeroContrat == "C1");
            Assert.NotNull(contrat1);
            Assert.Equal(2, contrat1.Participants.Count);
            Assert.Equal(2, contrat1.Garanties.Count);

            var contrat2 = resultat.Find(l => l.NumeroContrat == "C2");
            Assert.NotNull(contrat2);
            Assert.Single(contrat2.Participants);
            Assert.Empty(contrat2.Garanties);
        }
    }
}
