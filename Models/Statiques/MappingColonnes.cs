namespace DCCR_SERVER.Models.Statiques
{
    public class MappingColonnes
    {
        public int id_mapping { set; get; }
        public string colonne_excel { set; get; }
        public string? colonne_bdd { set; get; }
        public string? table_bdd { set; get; }
        public int ordre {  set; get; }
        public string? nom_table_lookup { get; set; } 
        public string? colonne_cle_lookup { get; set; } 
        public bool obligatoire { get; set; }
        public string type_donnee { get; set; }
    }
}
