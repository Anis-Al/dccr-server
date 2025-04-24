using System;
using System.Collections.Generic;

namespace DCCR_SERVER.DTOs
{
    public class LoanPreviewDto
    {
        public string NumeroContrat { get; set; }
        public string DateDeclaration { get; set; }
        // Add other loan-level fields as needed
        public List<ParticipantPreviewDto> Participants { get; set; }
        public List<GarantiePreviewDto> Garanties { get; set; }
    }

    public class ParticipantPreviewDto
    {
        public string ParticipantCle { get; set; }
        public string ParticipantType { get; set; }
        public string ParticipantNif { get; set; }
        public string? ParticipantCli { get; set; }
        public string ParticipantRib { get; set; }
        // Add other participant fields as needed
    }

    public class GarantiePreviewDto
    {
        public string? TypeGarantie { get; set; }
        public string? MontantGarantie { get; set; }
        // Add other garantie fields as needed
    }
}
