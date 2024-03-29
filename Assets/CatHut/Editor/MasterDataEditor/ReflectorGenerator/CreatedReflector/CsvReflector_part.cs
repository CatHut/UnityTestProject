﻿#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using CatHut;

namespace CatHut
{
    public static partial class CsvReflector
    {

        public static void ApplyValuesToGame(string parentName, string ChildName, FormatedCsvData fcd)
        {
            switch (parentName)
            {
                case "Enemy":
                    ReflectEnemy(ChildName, fcd);
                    break;
                case "Player":
                    ReflectPlayer(ChildName, fcd);
                    break;

					default:
						break;
			}

		}

	}
}

#endif