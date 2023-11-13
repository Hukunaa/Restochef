using Runtime.Utility;
using UnityEditor;
using UnityEngine;

public class DataManagementTool : EditorWindow
{
    private Vector2 scrollPos;
    
    [MenuItem("Tools/DataManager")]
    private static void ShowWindow()
    {
        var window = GetWindow<DataManagementTool>();
        window.titleContent = new GUIContent("Data Manager");
        window.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (GUILayout.Button("Reset all data"))
        {
            DataLoader.ResetAllData();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Tutorial Data", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Reset tutorial data"))
        {
            DataLoader.ResetTutorialData();
        }
        
        if (GUILayout.Button("Set All tutorial data complete"))
        {
            DataLoader.SetAllTutorialDataComplete();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Player Data", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Reset Player name"))
        {
            DataLoader.ResetPlayerNameData();
        }
        
        if (GUILayout.Button("Reset Kitchen Layout data"))
        {
            DataLoader.ResetKitchenLayoutData();
        }
        
        if (GUILayout.Button("Reset Player currencies data"))
        {
            DataLoader.ResetPlayerCurrenciesData();
        }
        
        if (GUILayout.Button("Reset Player Inventory data"))
        {
            DataLoader.ResetPlayerInventoryData();
        }
        
        if (GUILayout.Button("Reset Player Recipes data"))
        {
            DataLoader.ResetPlayerRecipesData();
        }
        
        if (GUILayout.Button("Reset Player Score data"))
        {
            DataLoader.ResetPlayerScoreData();
        }
        
        if (GUILayout.Button("Reset Player Kitchen data"))
        {
            DataLoader.ResetPlayerKitchenData();
        }
        
        if (GUILayout.Button("Reset Star Progress data"))
        {
            DataLoader.ResetStarProgressData();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Leaderboard Data", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset Leaderboard data"))
        {
            DataLoader.ResetLeaderboardData();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Store Data", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset Store Recipes data"))
        {
            DataLoader.ResetStoreRecipesData();
        }
        
        if (GUILayout.Button("Reset Store Chegs data"))
        {
            DataLoader.ResetStoreChefsData();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Tables", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset Chef Prices table data"))
        {
            DataLoader.ResetChefPricesTableData();
        }
        if (GUILayout.Button("Reset Recipes Prices table data"))
        {
            DataLoader.ResetRecipePricesTableData();
        }
        if (GUILayout.Button("Reset Chef levels table data"))
        {
            DataLoader.ResetChefsLevelTableData();
        }
        if (GUILayout.Button("Reset Station Stats table data"))
        {
            DataLoader.ResetStationStatsTableData();
        }
        if (GUILayout.Button("Reset Chef rarity table data"))
        {
            DataLoader.ResetChefRarityStatTableData();
        }

        EditorGUILayout.EndScrollView();
    }
}
