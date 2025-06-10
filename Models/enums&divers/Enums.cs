namespace DCCR_SERVER.Models.enums
{
    public class Enums
    {

        public enum RoleUtilisateur
        {
            admin = 1,
            integrateurExcel = 2,
            modificateurCredits = 3,
            generateurDeclarations = 4
        }

        public enum typeActionAudit
        {
            chargementExcel = 1,
            validationExcel = 2,
            creationCredit = 3,
            modificationCredit = 4,
            suppressionCredit = 5,
            generationXml= 6
        }
        public enum TypeFichierXml
        {
            correction=1,
            suppression=2
        }
        public enum StatutImport
        {
            Telechargement = 1,
            PretPourConfirmation = 2,
            EchecValidation = 3,
            ImportConfirme = 4
        }
    }
}
