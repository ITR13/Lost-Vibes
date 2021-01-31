#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LostVibes
{
    public partial class GameManager : MonoBehaviour
    {
        [SerializeField] public Camera camera;
        [SerializeField] public float fromHue, toHue;

        partial void AfterUpdate()
        {
            camera.backgroundColor = Color.HSVToRGB(
                Mathf.Lerp(fromHue, toHue, _chillTensity),
                1,
                0.5f
            );
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.FloatField("Chilltensity", _chillTensity);
            EditorGUILayout.FloatField("Velocity", _ctVelocity);
            EditorGUILayout.EndHorizontal();
            if (_ctIsBound)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.FloatField("Min Bound", _ctBoundPosition - boundRange);
                EditorGUILayout.FloatField("Max Bound", _ctBoundPosition + boundRange);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(GUI.skin.label.lineHeight + 5);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.FloatField("Bias", _ctBias);
            EditorGUILayout.FloatField("Cooldown", _vibeCooldown);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(_vibeCooldown > 0);
            EditorGUILayout.BeginHorizontal();
            foreach (var vibe in _vibes.Take(3))
            {
                if (GUILayout.Button($"{vibe.Name}"))
                {
                    ExecuteVibe(vibe);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            foreach (var vibe in _vibes.Skip(3))
            {
                if (GUILayout.Button($"{vibe.Name}"))
                {
                    ExecuteVibe(vibe);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Silence"))
            {
                _vibeCooldown = 1;
                _ctBias *= 0.7f;
                foreach (var key in _vibeInfluence.Keys.ToArray())
                {
                    _vibeInfluence[key] = Mathf.MoveTowards(_vibeInfluence[key], 1, 0.3f);
                }
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndArea();
        }
    }
}

#endif