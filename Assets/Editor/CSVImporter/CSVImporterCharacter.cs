using UnityEngine;
using UnityEditor;
using System.IO;
using Simulation.Enums.Character;

public class CSVImporterCharacter
{
    [MenuItem("Tools/Import CSV/Character")]
    public static void ImportCharactersFromCSV()
    {
        string assetFolder = "Assets/Addressables/Characters/Data";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Character CSV File", defaultPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No CSV file selected.");
            return;
        }

        AssetDatabaseManager.CreateFolderFromPath(assetFolder);

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV file does not contain enough lines.");
            return;
        }

        // Get CSV header index mapping
        string[] headers = lines[0].Split(',');

        int characterIdIndex        = System.Array.IndexOf(headers, "CharacterId");
        int bodyToneIndex        = System.Array.IndexOf(headers, "BodyTone");
        int portraitSizeIndex       = System.Array.IndexOf(headers, "PortraitSize");
        int characterSizeIndex      = System.Array.IndexOf(headers, "CharacterSize");
        int genderIndex             = System.Array.IndexOf(headers, "Gender");
        int elementIndex            = System.Array.IndexOf(headers, "Element");
        int positionIndex           = System.Array.IndexOf(headers, "Position");

        int hpIndex             = System.Array.IndexOf(headers, "Gp");
        int spIndex             = System.Array.IndexOf(headers, "Tp");
        int kickIndex           = System.Array.IndexOf(headers, "Kick");
        int controlIndex        = System.Array.IndexOf(headers, "Control");
        int techniqueIndex      = System.Array.IndexOf(headers, "Technique");
        int pressureIndex       = System.Array.IndexOf(headers, "Pressure");
        int physicalIndex       = System.Array.IndexOf(headers, "Physical");
        int agilityIndex        = System.Array.IndexOf(headers, "Agility");
        int intelligenceIndex   = System.Array.IndexOf(headers, "Intelligence");

        int moveId0Index  = System.Array.IndexOf(headers, "MoveId0");
        int moveLv0Index  = System.Array.IndexOf(headers, "MoveLv0");
        int moveId1Index  = System.Array.IndexOf(headers, "MoveId1");
        int moveLv1Index  = System.Array.IndexOf(headers, "MoveLv1");
        int moveId2Index  = System.Array.IndexOf(headers, "MoveId2");
        int moveLv2Index  = System.Array.IndexOf(headers, "MoveLv2");
        int moveId3Index  = System.Array.IndexOf(headers, "MoveId3");
        int moveLv3Index  = System.Array.IndexOf(headers, "MoveLv3");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();

            characterData.CharacterId   = values[characterIdIndex].Trim();
            characterData.BodyTone   = values[bodyToneIndex].Trim();
            characterData.PortraitSize  = EnumManager.StringToEnum<PortraitSize>(values[portraitSizeIndex].Trim());
            characterData.CharacterSize = EnumManager.StringToEnum<CharacterSize>(values[characterSizeIndex].Trim());
            characterData.Gender        = EnumManager.StringToEnum<Gender>(values[genderIndex].Trim());
            characterData.Element       = EnumManager.StringToEnum<Element>(values[elementIndex].Trim());
            characterData.Position      = EnumManager.StringToEnum<Position>(values[positionIndex].Trim());

            characterData.Gp            = int.Parse(values[hpIndex].Trim());
            characterData.Tp            = int.Parse(values[spIndex].Trim());
            characterData.Kick          = int.Parse(values[kickIndex].Trim());
            characterData.Control       = int.Parse(values[controlIndex].Trim());
            characterData.Technique     = int.Parse(values[techniqueIndex].Trim());
            characterData.Pressure      = int.Parse(values[pressureIndex].Trim());
            characterData.Physical      = int.Parse(values[physicalIndex].Trim());
            characterData.Agility       = int.Parse(values[agilityIndex].Trim());
            characterData.Intelligence  = int.Parse(values[intelligenceIndex].Trim());

            characterData.MoveIds = new string[4]
            {
                values[moveId0Index].Trim(),
                values[moveId1Index].Trim(),
                values[moveId2Index].Trim(),
                values[moveId3Index].Trim()
            };
            characterData.MoveLvs = new int[4]
            {
                int.Parse(values[moveLv0Index].Trim()),
                int.Parse(values[moveLv1Index].Trim()),
                int.Parse(values[moveLv2Index].Trim()),
                int.Parse(values[moveLv3Index].Trim())
            };

            string safeName = characterData.CharacterId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(characterData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Character ScriptableObjects created from CSV.");
    }
}
