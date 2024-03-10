using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CatHut;

//ScriptableObjectClass Named from FileName
public class Enemy : ScriptableObject
{

    public enum ATTR{
        NONE = 0,
        FIRE = 1,
        WIND = 2,
        WATER = 3
    }

    public enum PATTERN{
        LOOP = 0,
        RANDOM = 1
    }



    //Each Sheets ClassDaclare
    [SerializeField]
    private EnemyParameterDictionary _EnemyParameterData;
    public EnemyParameterDictionary EnemyParameterData
    {
        get { return _EnemyParameterData; }
        set { _EnemyParameterData = value; } 
    }

    [SerializeField]
    private SkillPatternDictionary _SkillPatternData;
    public SkillPatternDictionary SkillPatternData
    {
        get { return _SkillPatternData; }
        set { _SkillPatternData = value; } 
    }

    public object this[string propertyName]
    {
        get
        {
            return typeof(Enemy).GetProperty(propertyName).GetValue(this);
        }

        set
        {
            typeof(Enemy).GetProperty(propertyName).SetValue(this, value);
        }
    }




    public List<string> PropertyNames
    {
        get
        {
            var ret = new List<string>();
            var properties = this.GetType().GetProperties()
                .Where(p => p.PropertyType != typeof(System.Object) && p.Name != "PropertyNames" && p.DeclaringType == this.GetType())
                .ToArray();
            // インデクサによるItemプロパティ(System.Ojbect)を除外
            // 基底クラスのプロパティを除外

            foreach (var property in properties)
            {
                ret.Add(property.Name);
            }

            return ret;
        }
    }



[System.Serializable]
public class EnemyParameterDictionary : SerializableDictionary<string, EnemyParameter> { }
[System.Serializable]
public class SkillPatternDictionary : SerializableDictionary<string, SkillPattern> { }



[System.Serializable]
public class EnemyParameter : IMasterData
{
    [SerializeField]
    private string _id = "";
    public string id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField]
    private string _NAME = "";
    public string NAME
    {
        get { return _NAME; }
        set { _NAME = value; }
    }

    [SerializeField]
    private int _LV;
    public int LV
    {
        get { return _LV; }
        set { _LV = value; }
    }

    [SerializeField]
    private Enemy.ATTR _attr;
    public Enemy.ATTR attr
    {
        get { return _attr; }
        set { _attr = value; }
    }

    [SerializeField]
    private int _HP;
    public int HP
    {
        get { return _HP; }
        set { _HP = value; }
    }

    [SerializeField]
    private int _MP;
    public int MP
    {
        get { return _MP; }
        set { _MP = value; }
    }

    [SerializeField]
    private int _ATK;
    public int ATK
    {
        get { return _ATK; }
        set { _ATK = value; }
    }

    [SerializeField]
    private int _DEF;
    public int DEF
    {
        get { return _DEF; }
        set { _DEF = value; }
    }

    [SerializeField]
    private int _INT;
    public int INT
    {
        get { return _INT; }
        set { _INT = value; }
    }

    [SerializeField]
    private int _REG;
    public int REG
    {
        get { return _REG; }
        set { _REG = value; }
    }

    [SerializeField]
    private int _SPD;
    public int SPD
    {
        get { return _SPD; }
        set { _SPD = value; }
    }

    [SerializeField]
    private int _EXP;
    public int EXP
    {
        get { return _EXP; }
        set { _EXP = value; }
    }

    [SerializeField]
    private string _IMAGE = "";
    public string IMAGE
    {
        get { return _IMAGE; }
        set { _IMAGE = value; }
    }

    [SerializeField]
    private int _lowlimit;
    public int lowlimit
    {
        get { return _lowlimit; }
        set { _lowlimit = value; }
    }

    [SerializeField]
    private int _uplimit;
    public int uplimit
    {
        get { return _uplimit; }
        set { _uplimit = value; }
    }

    [SerializeField]
    private bool _BOSS;
    public bool BOSS
    {
        get { return _BOSS; }
        set { _BOSS = value; }
    }

    [SerializeField]
    private string _Pattern = "";
    public string Pattern
    {
        get { return _Pattern; }
        set { _Pattern = value; }
    }

    public object this[string propertyName]
    {
       get
       {
            return typeof(EnemyParameter).GetProperty(propertyName).GetValue(this);
        }

       set
        {
            typeof(EnemyParameter).GetProperty(propertyName).SetValue(this, value);
        }
    }


    public List<string> PropertyNames
    {
       get
       {
            var ret = new List<string>();
            var properties = this.GetType().GetProperties()
                .Where(p => p.PropertyType != typeof(System.Object) && p.Name != "PropertyNames")
                .ToArray();  //インデクサによるItemプロパティ(System.Ojbect)を除外

            foreach (var property in properties)
            {
                 ret.Add(property.Name);
            }

        return ret;
        }
    }


}

[System.Serializable]
public class SkillPattern : IMasterData
{
    [SerializeField]
    private string _id = "";
    public string id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField]
    private Enemy.PATTERN _Pattern;
    public Enemy.PATTERN Pattern
    {
        get { return _Pattern; }
        set { _Pattern = value; }
    }

    [SerializeField]
    private int _skill1;
    public int skill1
    {
        get { return _skill1; }
        set { _skill1 = value; }
    }

    [SerializeField]
    private int _skill2;
    public int skill2
    {
        get { return _skill2; }
        set { _skill2 = value; }
    }

    [SerializeField]
    private int _skill3;
    public int skill3
    {
        get { return _skill3; }
        set { _skill3 = value; }
    }

    [SerializeField]
    private int _skill4;
    public int skill4
    {
        get { return _skill4; }
        set { _skill4 = value; }
    }

    [SerializeField]
    private int _skill5;
    public int skill5
    {
        get { return _skill5; }
        set { _skill5 = value; }
    }

    [SerializeField]
    private int _weight1;
    public int weight1
    {
        get { return _weight1; }
        set { _weight1 = value; }
    }

    [SerializeField]
    private int _weight2;
    public int weight2
    {
        get { return _weight2; }
        set { _weight2 = value; }
    }

    [SerializeField]
    private int _weight3;
    public int weight3
    {
        get { return _weight3; }
        set { _weight3 = value; }
    }

    [SerializeField]
    private int _weight4;
    public int weight4
    {
        get { return _weight4; }
        set { _weight4 = value; }
    }

    [SerializeField]
    private int _weight5;
    public int weight5
    {
        get { return _weight5; }
        set { _weight5 = value; }
    }

    public object this[string propertyName]
    {
       get
       {
            return typeof(SkillPattern).GetProperty(propertyName).GetValue(this);
        }

       set
        {
            typeof(SkillPattern).GetProperty(propertyName).SetValue(this, value);
        }
    }


    public List<string> PropertyNames
    {
       get
       {
            var ret = new List<string>();
            var properties = this.GetType().GetProperties()
                .Where(p => p.PropertyType != typeof(System.Object) && p.Name != "PropertyNames")
                .ToArray();  //インデクサによるItemプロパティ(System.Ojbect)を除外

            foreach (var property in properties)
            {
                 ret.Add(property.Name);
            }

        return ret;
        }
    }


}



}