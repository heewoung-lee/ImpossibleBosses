using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GameManagers.Interface.DataManager
{
    public interface IGoogleDataBaseStruct
    {
        public GoogleDataBaseStruct DataBaseStruct { get; }
        public Spreadsheet GetGoogleSpreadsheet(GoogleDataBaseStruct databaseStruct,out SheetsService service,out string spreadsheetId,bool isWrite = false);
    }
}
