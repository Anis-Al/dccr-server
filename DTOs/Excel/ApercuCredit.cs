using System;
using System.Collections.Generic;

namespace DCCR_SERVER.DTOs.Excel
{
    public class ApercuCredit
    {

        public string NumeroContrat { get; set; }
        public string DateDeclaration { get; set; }
        public string SituationCredit { get; set; }
        public string DateOctroi { get; set; }
        public string DateRejet { get; set; }
        public string DateExpiration { get; set; }
        public string DateExecution { get; set; }
        public string DureeInitiale { get; set; }
        public string DureeRestante { get; set; }
        public string TypeCredit { get; set; }
        public string ActiviteCredit { get; set; }
        public string Monnaie { get; set; }
        public string CreditAccorde { get; set; }
        public string IdPlafond { get; set; }
        public string Taux { get; set; }
        public string Mensualite { get; set; }
        public string CoutTotalCredit { get; set; }
        public string SoldeRestant { get; set; }
        public string ClasseRetard { get; set; }
        public string DateConstatation { get; set; }
        public string NombreEcheancesImpayes { get; set; }
        public string MontantInteretsCourus { get; set; }
        public string MontantInteretsRetard { get; set; }
        public string MontantCapitalRetard { get; set; }
        public string Motif { get; set; }
        public string CodeAgence { get; set; }
        public string CodeWilaya { get; set; }
        public string CodePays { get; set; }
        public bool EstValide { get; set; }
        public List<ApercuParticipant> Participants { get; set; }
        public List<ApercuGarantie> Garanties { get; set; }
    }

    public class ApercuParticipant
    {
        public string ParticipantCle { get; set; }
        public string ParticipantType { get; set; }
        public string ParticipantNif { get; set; }
        public string? ParticipantCli { get; set; }
        public string ParticipantRib { get; set; }
        public string RoleNiveauResponsabilite { get; set; }
    }

    public class ApercuGarantie
    {
        public string? TypeGarantie { get; set; }
        public string? MontantGarantie { get; set; }
    }
}
