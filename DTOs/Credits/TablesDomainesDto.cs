namespace DCCR_SERVER.DTOs.Credits
{
    public class TablesDomainesDto
    {
        public string nom_table { get; set; }
        public List<ValeursTableDomaines> valeurs { get; set; }
    }
    public class ValeursTableDomaines
    {
        public string code { get; set; }
        public string domaine {  get; set; }
    }
}
