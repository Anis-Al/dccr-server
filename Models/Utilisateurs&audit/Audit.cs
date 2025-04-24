using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using static DCCR_SERVER.Models.enums.Enums;

namespace DCCR_SERVER.Models.Utilisateurs_audit
{
    public class Audit
    {
        public int id_action { get; set; }
        public string matricule_utilisateur { get; set; }
        public required string table_ciblée { get; set; }
        public required string? id_entité { get; set; }
        public required typeActionAudit type_action { get; set; }
        public DateTime date_action { get; set; } = DateTime.Now;
        public Utilisateur utilisateur_acteur { get; set; }

    }
}
