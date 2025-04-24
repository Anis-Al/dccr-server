namespace DCCR_SERVER.DTOs
{
    public class memoireDTO
    {
        public class ResultatMiseEnAttenteDto { 
            public bool Success { get; set; } 
            public Guid? UploadSessionId { get; set; } 
            public int ExcelImportId { get; set; } 
            public string Message { get; set; } 
            public int TotalRowsProcessed { get; set; } 
            public int RowsWithErrors { get; set; } 
        }
        public class ImportResult { 
            public bool Success { get; set; } 
            public string Message { get; set; } 
            public int LoansImported { get; set; } 
            public int RolesImported { get; set; } 
            public int GuaranteesImported { get; set; } 
        }
        public class DiscardRequest { 
            public Guid? UploadSessionId { get; set; } 
            public int? ExcelImportId { get; set; } 
        }
        public class ImportErrorDto { 
            public int? RowNumber { get; set; } 
            public string FieldName { get; set; } 
            public string NumeroContrat { get; set; } 
            public string ErrorMessage { get; set; } 
            public string OffendingValue { get; set; } 
        }

    }
}

