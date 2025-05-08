using System.Collections.Generic;

namespace DCCR_SERVER.Models.Principaux
{
    public class TableauDeBord
    {
        public int id_kpi { get; set; }
        public string description_kpi { get; set; }
        public string requete_sql { get; set; }
    }

   
}