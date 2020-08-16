using Google.GData.Spreadsheets;
using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheetsBridge : MonoBehaviour
{
    public readonly string sheetId = "1iZsNubFB2pePT0MlqBUyMg4la7p5V5hr6ER3WzXQ4Ww"; //The sheet Id to read
    public readonly string worksheetName = "Niveaux"; //the worksheet to read
    public readonly string startCell = "A4";
    public readonly string endCell = "Z145";

    // Start is called before the first frame update
    void Start()
    {
        //SpreadsheetManager manager = new SpreadsheetManager();
        Debug.Log("Searching");SpreadsheetManager.Read(new GSTU_Search(sheetId, worksheetName, startCell, endCell), ReadAllLevelNames, false);
    }

    void ReadAllLevelNames(GstuSpreadSheet spreadsheetRef)
    {
        Debug.Log("Searching");
        Debug.Log(spreadsheetRef["D30"].value);

        for (int i = 6; i < 33; i++)
        {
            Debug.Log(spreadsheetRef["D" + i].value);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
