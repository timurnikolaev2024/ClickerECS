using System.IO;
using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Save
{
    public static class SaveService
    {
        private const string FileName = "clicker_save.json";

        [System.Serializable]
        private class SaveData
        {
            public float balance;
            public int[] levels;
            public float[] progress;
            public bool[] upgrade1;
            public bool[] upgrade2;
        }

        public static void Save(EcsWorld world, GameCatalogSO catalog, int balanceEnt)
        {
            var data = new SaveData
            {
                balance = world.GetPool<Balance>().Get(balanceEnt).Value,
                levels = new int[catalog.Businesses.Length],
                progress = new float[catalog.Businesses.Length],
                upgrade1 = new bool[catalog.Businesses.Length],
                upgrade2 = new bool[catalog.Businesses.Length]
            };

            var id  = world.GetPool<BusinessId>();
            var lvl = world.GetPool<Level>();
            var prog = world.GetPool<Progress>();
            var up1 = world.GetPool<Upgrade1PurchasedTag>();
            var up2 = world.GetPool<Upgrade2PurchasedTag>();

            foreach (int e in world.Filter<BusinessId>().End()) 
            {
                int i = id.Get(e).Id;
                data.levels[i] = lvl.Get(e).Value;
                data.progress[i] = prog.Get(e).Value;
                data.upgrade1[i] = up1.Has(e);
                data.upgrade2[i] = up2.Has(e);
            }

            File.WriteAllText(Path.Combine(Application.persistentDataPath, FileName), JsonUtility.ToJson(data));
        }

        public static void Load(EcsWorld world, GameCatalogSO catalog, int balanceEnt)
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            
            if (!File.Exists(path)) 
                return;

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            world.GetPool<Balance>().Get(balanceEnt).Value = data.balance;

            var id  = world.GetPool<BusinessId>();
            var lvl = world.GetPool<Level>();
            var prog = world.GetPool<Progress>();
            var up1 = world.GetPool<Upgrade1PurchasedTag>();
            var up2 = world.GetPool<Upgrade2PurchasedTag>();

            foreach (int e in world.Filter<BusinessId>().End()) 
            {
                int i = id.Get(e).Id;
                lvl.Get(e).Value = data.levels[i];
                prog.Get(e).Value = data.progress[i];
                
                if (data.upgrade1[i] && !up1.Has(e)) 
                    up1.Add(e);
                if (data.upgrade2[i] && !up2.Has(e)) 
                    up2.Add(e);
            }
        }
        
        public static void Clear()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"<color=red>[SaveService]</color> Save file deleted: {path}");
            }
        }
    }
}