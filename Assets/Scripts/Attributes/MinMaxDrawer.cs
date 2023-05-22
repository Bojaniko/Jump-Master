using UnityEngine;
using UnityEditor;

namespace Studio28.Attributes
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 48f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.propertyType.Equals(SerializedPropertyType.Vector2))
            {
                EditorGUI.LabelField(position, "A min max slider must be a vector2.");
                return;
            }

            MinMaxAttribute mmatr = attribute as MinMaxAttribute;

            float val_min = property.vector2Value.x;
            float val_max = property.vector2Value.y;
            float lim_min = mmatr.MinValue;
            float lim_max = mmatr.MaxValue;

            position.y += 2;

            Rect rec_pos = position;
            rec_pos.height = 47;
            EditorGUI.DrawRect(rec_pos, new Color(0.87f, 0.87f, 0.87f));

            position.width -= 10;
            position.x += 5;

            Rect slide_pos = position;
            slide_pos.height = 15;
            EditorGUI.MinMaxSlider(slide_pos, label, ref val_min, ref val_max, lim_min, lim_max);

            Rect vec_pos = position;
            vec_pos.y += 25;
            Vector2 final = EditorGUI.Vector2Field(vec_pos, "", new Vector2(float.Parse(val_min.ToString("0.00")), float.Parse(val_max.ToString("0.00"))));

            property.vector2Value = final;
        }
    }
}