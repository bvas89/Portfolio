using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public partial class WeaponStats : ScriptableObject
{
    // variables
    protected string _weaponName;
    protected float _damage;
    protected float _cooldown;

    protected HitAreaType _attackType;
    protected Sprite _sprite;
    protected GameObject _prefab;
    protected Animation[] _animations;

    // if Custom
    protected float _length;
    protected float _radius;

    public Weapon GetWeapon()
    {
        Weapon w = new Weapon();

        w.Name = _weaponName;
        w.Damage = _damage;
        w.Cooldown = _cooldown;
        w.Prefab = _prefab;
        w.Sprite = _sprite;

        switch(_attackType)
        {
            case HitAreaType.Arc:
                w.Length = 5f;
                w.Radius = 180f;
                break;
            case HitAreaType.Cone:
                w.Length = 7f;
                w.Radius = 90f;
                break;
            case HitAreaType.Circle:
                w.Length = 3f;
                w.Radius = 360f;
                break;
            case HitAreaType.Custom:
                w.Length = _length;
                w.Radius = _radius;
                break;
        }

        return w;
    }
}

public class Weapon
{
    public string Name;
    public float Damage;
    public float Cooldown;
    public float Length;
    public float Radius;
    public Sprite Sprite;
    public GameObject Prefab;
    public Animation[] Animations;

    //TODO: Swing Animation
}

public enum HitAreaType
{
    Arc, Cone, Circle, Custom
}

//Editor
public partial class WeaponStats
{
    [CustomEditor(typeof(WeaponStats))]
    public class WeaponEditor : Editor
    {
        public override void OnInspectorGUI()
        { 
            WeaponStats wpn = (WeaponStats)target;

            wpn._weaponName = EditorGUILayout.TextField("Name", wpn._weaponName);

            //Changing the name Renames the file.
            string assetPath = AssetDatabase.GetAssetPath(wpn);
            string fileName = "WPN - " + wpn._weaponName;
            AssetDatabase.RenameAsset(assetPath, fileName);
            //AssetDatabase.Refresh();

            wpn._damage = EditorGUILayout.FloatField("Damage", wpn._damage);
            wpn._cooldown = EditorGUILayout.FloatField("Cooldown", wpn._cooldown);
            wpn._prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", wpn._prefab, typeof(GameObject), true);
            wpn._attackType = (HitAreaType)EditorGUILayout.EnumPopup("Hit Area Type", wpn._attackType);

            if (wpn._attackType == HitAreaType.Custom)
            {
                wpn._radius = EditorGUILayout.FloatField("Radius", wpn._radius);
                wpn._length = EditorGUILayout.FloatField("Length", wpn._length);
            }

            wpn._sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", wpn._sprite, typeof(Sprite), true);

            // Array Example
            {
                /* -------------
                // ARRAY EXAMPLE

                if (wpn._animations == null || wpn._animations.Length < 5)
                {
                    wpn._animations = new Animation[3];
                }

                for (int i = 0; i < wpn._animations.Length; i++)
                {
                    wpn._animations[i] = (Animation)EditorGUILayout.ObjectField("ABC", wpn._animations[i], typeof(Animation), true);
                }
                */
            }
        }
    }
}


