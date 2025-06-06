#if UNITY_EDITOR
using Game.Scripts.Save;
using UnityEditor;

namespace Game.Editor
{
    public static class SaveToolEditor
    {
        [MenuItem("Tools/Save/Clear Save Data")]
        private static void ClearFromMenu()
        {
            SaveService.Clear();
        }
    }
}
#endif