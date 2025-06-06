namespace DCCR_SERVER.Models.ValidationFichiers
{
    public class RegleValidation
    {
        public int id_regle { get; set; }
        public string nom_colonne { get; set; }
        public string type_regle { get; set; }
        public string? valeur_regle { get; set; } 
        public string message_erreur { get; set; }

        public string? colonne_dependante { get; set; } // Colonne déclencheur
        public string? valeur_dependante { get; set; } // Valeur déclencheur
        public string? colonne_cible { get; set; } // Colonne cible à vérifier
        public string? valeur_cible_attendue { get; set; } // Valeur attendue dans la colonne cible

        public List<ErreurExcel> erreurs { get; set; }
    }
}
