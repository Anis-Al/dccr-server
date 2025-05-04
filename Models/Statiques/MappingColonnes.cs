namespace DCCR_SERVER.Models.Statiques
{
    public class MappingColonnes
    {
        public int id_mapping { set; get; }
        public string colonne_excel { set; get; }
        public string? colonne_bdd { set; get; }
        public string? table_prod { set; get; }
        public string type_donnee_prod { get; set; }
    }
}
