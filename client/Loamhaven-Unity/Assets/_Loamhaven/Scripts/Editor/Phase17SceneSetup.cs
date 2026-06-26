using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Loamhaven.World;

namespace Loamhaven.Editor
{
    public static class Phase17SceneSetup
    {
        [MenuItem("Loamhaven/Auto-Setup Phase 17 Scene")]
        static void Setup()
        {
            var root = GameObject.Find("GameManagers");
            if (root == null)
            {
                root = new GameObject("GameManagers");
                Undo.RegisterCreatedObjectUndo(root, "Create GameManagers");
                Debug.Log("[Phase17Setup] Created GameManagers.");
            }
            else
            {
                Debug.Log("[Phase17Setup] GameManagers already exists — reusing.");
            }

            EnsureChild<VFXSystem>(root, "VFXSystem");
            EnsureChild<TorchGlowSystem>(root, "TorchGlowSystem");

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            Debug.Log("[Phase17Setup] Done. Save the scene (Ctrl+S) to persist.");
            EditorUtility.DisplayDialog(
                "Phase 17 Scene Setup",
                "GameManagers hierarchy created:\n\n" +
                "  GameManagers\n" +
                "  ├─ VFXSystem        (VFXSystem)\n" +
                "  └─ TorchGlowSystem  (TorchGlowSystem)\n\n" +
                "Save the scene now to keep these changes.",
                "OK");
        }

        static T EnsureChild<T>(GameObject parent, string childName) where T : Component
        {
            var existing = parent.transform.Find(childName);
            if (existing != null)
            {
                var comp = existing.GetComponent<T>();
                if (comp == null)
                {
                    comp = existing.gameObject.AddComponent<T>();
                    Undo.RegisterCreatedObjectUndo(existing.gameObject, $"Add {typeof(T).Name}");
                    Debug.Log($"[Phase17Setup] Added {typeof(T).Name} to existing '{childName}'.");
                }
                else
                {
                    Debug.Log($"[Phase17Setup] '{childName}' with {typeof(T).Name} already exists — skipped.");
                }
                return comp;
            }

            var go = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(go, $"Create {childName}");
            go.transform.SetParent(parent.transform, false);
            var c = go.AddComponent<T>();
            Debug.Log($"[Phase17Setup] Created '{childName}' with {typeof(T).Name}.");
            return c;
        }

        [MenuItem("Loamhaven/Auto-Setup Phase 17 Scene", validate = true)]
        static bool ValidateSetup()
        {
            return !string.IsNullOrEmpty(SceneManager.GetActiveScene().name);
        }
    }
}