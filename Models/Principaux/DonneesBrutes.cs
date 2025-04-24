using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DCCR_SERVER.Models.Principaux
{
    
    public class donnees_brutes
    {
        public int id { get; set; }

        public int id_import_excel { get; set; } 
        public Guid id_session_import { get; set; }
        public int ligne_original { get; set; }

        public string? numero_contrat { get; set; }
        public string? date_declaration { get; set; }

        [ChampCoherence]
        public string? situation_credit { get; set; }
        [ChampCoherence]
        public string? date_octroi { get; set; }
        [ChampCoherence]
        public string? date_rejet { get; set; }
        [ChampCoherence]
        public string? date_expiration { get; set; }
        [ChampCoherence]
        public string? date_execution { get; set; }
        [ChampCoherence]
        public string? duree_initiale { get; set; }
        [ChampCoherence]
        public string? duree_restante { get; set; }
        [ChampCoherence]
        public string? type_credit { get; set; }
        [ChampCoherence]
        public string? activite_credit { get; set; }
        [ChampCoherence]
        public string? monnaie { get; set; }
        [ChampCoherence]
        public string? credit_accorde { get; set; }
        //[ChampCoherence]
        //public bool est_plafond_accorde { get; set; } = true;

        [ChampCoherence]
        public string? id_plafond { get; set; }
        [ChampCoherence]
        public string? taux { get; set; }
        [ChampCoherence]
        public string? mensualite { get; set; }
        [ChampCoherence]
        public string? cout_total_credit { get; set; }
        [ChampCoherence]
        public string? solde_restant { get; set; }

        [ChampCoherence]
        public string? classe_retard { get; set; }
        [ChampCoherence]
        public string? date_constatation { get; set; }
        [ChampCoherence]
        public string? nombre_echeances_impayes { get; set; }
        [ChampCoherence]
        public string? montant_interets_courus { get; set; }
        [ChampCoherence]
        public string? montant_interets_retard { get; set; }
        [ChampCoherence]
        public string? montant_capital_retard { get; set; }
        [ChampCoherence]
        public string? motif { get; set; }



        public string? participant_cle { get; set; }
        [ChampCoherenceParticipant]
        public string? participant_type_cle { get; set; }
        [ChampCoherenceParticipant]
        public string? participant_nif { get; set; }
        [ChampCoherenceParticipant]
        public string? participant_cli { get; set; }
        [ChampCoherenceParticipant]
        public string? participant_rib { get; set; }


        public string? role_niveau_responsabilite { get; set; }


        public string? garantie_type_garantie { get; set; }
        public string? garantie_montant_garantie { get; set; }

        [ChampCoherence]
        public string? code_agence { get; set; }   
        [ChampCoherence]          
        public string? code_wilaya { get; set; } 
        [ChampCoherence]
        public string? code_pays { get; set; }       

        public bool est_valide { get; set; } = true;
        
        public string messages_validation { get; set; } 

        // Navigation
        public FichierExcel import_excel { get; set; }
    }
}
