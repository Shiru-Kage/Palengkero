using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterData characterData = (CharacterData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Preferences", EditorStyles.boldLabel);
        
        int cheapItemsChance = characterData.preferCheapItemsChance;
        int highNutritionChance = characterData.preferHighNutritionChance;
        int highSatisfactionChance = characterData.preferHighSatisfactionChance;

        EditorGUI.BeginChangeCheck();

        cheapItemsChance = EditorGUILayout.IntSlider("Prefer Cheap Items", cheapItemsChance, 0, 100);
        highNutritionChance = EditorGUILayout.IntSlider("Prefer High Nutrition", highNutritionChance, 0, 100);
        highSatisfactionChance = EditorGUILayout.IntSlider("Prefer High Satisfaction", highSatisfactionChance, 0, 100);

        if (EditorGUI.EndChangeCheck())
        {
            int total = cheapItemsChance + highNutritionChance + highSatisfactionChance;

            if (total > 100)
            {
                int excess = total - 100;

                if (highSatisfactionChance >= highNutritionChance)
                {
                    highSatisfactionChance -= Mathf.Min(excess, highSatisfactionChance);
                }
                else
                {
                    highNutritionChance -= Mathf.Min(excess, highNutritionChance);
                }
            }
            else if (total < 100)
            {
                int deficit = 100 - total;

                if (highSatisfactionChance < 100)
                {
                    highSatisfactionChance += Mathf.Min(deficit, 100 - highSatisfactionChance);
                }
                else if (highNutritionChance < 100)
                {
                    highNutritionChance += Mathf.Min(deficit, 100 - highNutritionChance);
                }
                else if (cheapItemsChance < 100)
                {
                    cheapItemsChance += Mathf.Min(deficit, 100 - cheapItemsChance);
                }
            }

            characterData.preferCheapItemsChance = cheapItemsChance;
            characterData.preferHighNutritionChance = highNutritionChance;
            characterData.preferHighSatisfactionChance = highSatisfactionChance;
        }

        if (GUILayout.Button("Reset to 0"))
        {
            characterData.preferCheapItemsChance = 0;
            characterData.preferHighNutritionChance = 0;
            characterData.preferHighSatisfactionChance = 0;
        }
    }
}
