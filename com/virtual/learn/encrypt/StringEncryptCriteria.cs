namespace lug.String.Encrypt
{
    public class StringEncryptCriteria
    {
        public int KeySize {get;set;}
        public int DerivationIterations {get;set;}
        public string Text {get; set;} 
        public string PassPhrase {get; set;}
    }
}