using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;



public class MasterData : SingletonMonoBehaviour<MasterData>
{

    //Created MasterData
    private Enemy _EnemyData;
    public Enemy EnemyData
    {
        get { return _EnemyData; }
        set { _EnemyData = value; } 
    }

    private Player _PlayerData;
    public Player PlayerData
    {
        get { return _PlayerData; }
        set { _PlayerData = value; } 
    }



    private new void Awake()
    {
        base.Awake();

        EnemyData = Addressables.LoadAssetAsync<Enemy>("Enemy").WaitForCompletion();
        PlayerData = Addressables.LoadAssetAsync<Player>("Player").WaitForCompletion();


    }

    public object this[string propertyName]
    {
        get
        {
            return typeof(MasterData).GetProperty(propertyName).GetValue(this);
        }

        set
        {
            typeof(MasterData).GetProperty(propertyName).SetValue(this, value);
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
}

