using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Scripts.BundleHashCalculator
{
    [CustomEditor(typeof(BundleHashCalculator))]
    public class BundleHashCalculatorEditor : Editor
    {
        private BundleHashCalculator _hashBundleCalculator;

        private bool _isEnableSettings;
        private bool _isOperationSuccess = false;
        private bool _isCallOperation = false;

        private string _cachBundleName;
        private NameDeveloper _cachNameDeveloper;

        private void OnEnable()
        {
            _hashBundleCalculator = (BundleHashCalculator)target;
            _isOperationSuccess = false;
            _isCallOperation = false;
        }

        public override void OnInspectorGUI()
        {
            if (_cachBundleName != _hashBundleCalculator.NameBundle
                || _cachNameDeveloper != _hashBundleCalculator.SelectedDeveloperPreset)
            {
                _isOperationSuccess = false;
                _isCallOperation = false;
            }
            _cachBundleName = _hashBundleCalculator.NameBundle;
            _cachNameDeveloper = _hashBundleCalculator.SelectedDeveloperPreset;

            GUILayout.Space(10);

            var styleMainLabel = new GUIStyle(GUI.skin.label);
            styleMainLabel.normal.textColor = Color.white;
            styleMainLabel.fontSize = 20;
            styleMainLabel.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Hash Calculator", styleMainLabel);

            GUILayout.Space(10);

            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            _hashBundleCalculator.SelectedDeveloperPreset = (NameDeveloper)EditorGUILayout.EnumPopup("Developer preset", _hashBundleCalculator.SelectedDeveloperPreset);
            _hashBundleCalculator.NameBundle = EditorGUILayout.TextField("Bundle name", _hashBundleCalculator.NameBundle);

            bool isSelectPresetDeveloper = _hashBundleCalculator.SelectedDeveloperPreset != NameDeveloper.None;
            bool isExistNameBundle = !string.IsNullOrEmpty(_hashBundleCalculator.NameBundle);
            bool isExistPresetDeveloper = _hashBundleCalculator.CheckPresetDeveloper();
            bool isExistPathToFolderGit = _hashBundleCalculator.CheckPresetPathToGitFolder();

            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            EditorGUI.BeginDisabledGroup(!(isExistPathToFolderGit && isSelectPresetDeveloper && isExistNameBundle));
            {
                if (GUILayout.Button("Apply all", GUILayout.Height(30)))
                {
                    _isCallOperation = true;
                    _isOperationSuccess = _hashBundleCalculator.ApplyAll();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!isExistNameBundle);
            {
                GUI.backgroundColor = originalColor;
                if (GUILayout.Button("Calculate hash", GUILayout.Height(30)))
                {
                    _isCallOperation = true;
                    _isOperationSuccess = _hashBundleCalculator.CalculateHash();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!(isExistPathToFolderGit && isSelectPresetDeveloper && isExistNameBundle));
            {
                if (GUILayout.Button("Copy bundles to folder", GUILayout.Height(30)))
                {
                    _isCallOperation = true;
                    _isOperationSuccess = _hashBundleCalculator.CopyFileToFolderByPresetDeveloper();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (_isCallOperation)
            {
                var style = new GUIStyle(GUI.skin.label);
                style.fontSize = 12;
                style.alignment = TextAnchor.MiddleCenter;
                if (_isOperationSuccess)
                {
                    style.normal.textColor = Color.green;
                    EditorGUILayout.LabelField("Operation completed successfully!", style);
                }
                else
                {
                    style.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("Operation failed!", style);
                }
            }

            EditorGUILayout.Space();
            rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Settings", GUILayout.Height(30)))
            {
                _isEnableSettings = !_isEnableSettings;
            }

            if (_isEnableSettings)
            {
                var styleLabelSettings = new GUIStyle(GUI.skin.label);
                styleLabelSettings.normal.textColor = Color.white;
                styleLabelSettings.fontSize = 14;
                EditorGUILayout.LabelField("Adding developer presets", styleLabelSettings);

                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Developers"), true);
                serializedObject.ApplyModifiedProperties();

                _hashBundleCalculator.HashAndroid = EditorGUILayout.TextField("Android", _hashBundleCalculator.HashAndroid);
                _hashBundleCalculator.HashIOS = EditorGUILayout.TextField("IOS", _hashBundleCalculator.HashIOS);
                _hashBundleCalculator.HashWindows = EditorGUILayout.TextField("Windows", _hashBundleCalculator.HashWindows);
            }
        }
    }
}
