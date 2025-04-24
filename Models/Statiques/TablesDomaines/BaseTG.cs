using System.ComponentModel.DataAnnotations.Schema;

namespace DCCR_SERVER.Models.Statiques.TablesDomaines
{
    public class BaseTG
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public string code { get; set; }
        public required string domaine { get; set; }
    }
}
