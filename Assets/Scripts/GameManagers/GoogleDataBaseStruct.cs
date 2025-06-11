namespace GameManagers
{
    public struct GoogleDataBaseStruct
    {
        public GoogleDataBaseStruct(string googleClientID,string googleSecret,string applicationName,string spreedSheetID)
        {
            GoogleClientID = googleClientID;
            GoogleSecret = googleSecret;
            ApplicationName = applicationName;
            SpreedSheetID = spreedSheetID;
        }
        public readonly string GoogleClientID;
        public readonly string GoogleSecret;
        public readonly string ApplicationName;
        public string SpreedSheetID;
    }
}