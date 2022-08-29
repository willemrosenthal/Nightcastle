using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor {

    public static class SceneOrderMenu
    {
        [MenuItem(":)/Sort BuildSetting")]
        static void SortBuildSettings() {
            var scenes = EditorBuildSettings.scenes;
            var list = new List<EditorBuildSettingsScene>(scenes);
            var sortedScenes = list.OrderBy((e) => {
                return e.path;
            }).ToArray();
            EditorBuildSettings.scenes = sortedScenes;
        }

        // [MenuItem("MyMenu/Do Something")]
        // static void DoSomething()
        // {
        //     Debug.Log("Doing Something...");
        // }
    }

}
