namespace DCCR_SERVER.DTOs.Export
{
    public class ErreurExcelSimpleDto
    {
        public int id_excel { set;get; }
        public int ligne_excel { set; get; }
        public string message_erreur { set; get; }
    }
}
