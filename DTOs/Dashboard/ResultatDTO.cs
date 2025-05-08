using System.Collections.Generic;

namespace DCCR_SERVER.DTOs.Dashboard
{
    public class ResultatDTO<T>
    {
        public int id_kpi { get; set; }
        public string description_kpi { get; set; }
        public IEnumerable<T> resultats { get; set; }
    }
}