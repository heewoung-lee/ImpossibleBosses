public struct GoogleDataBaseStruct
{
    public GoogleDataBaseStruct(string google_client_ID,string google_Secret,string applicationName,string spreedSheetID)
    {
        _google_Client_ID = google_client_ID;
        _google_Secret = google_Secret;
        _applicationName = applicationName;
        _spreedSheetID = spreedSheetID;
    }
    public string _google_Client_ID;
    public string _google_Secret;
    public string _applicationName;
    public string _spreedSheetID;
}