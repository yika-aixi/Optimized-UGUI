using System;
using System.Linq;
using CabinIcarus.EditorFrame.Base.Editor;
using CabinIcarus.EditorFrame.Config;
using CabinIcarus.EditorFrame.Log;
using CabinIcarus.EditorFrame.Utils;
using UnityEditor;
using UnityEngine;

namespace CabinIcarus.OptimizedUGUI
{
    public class OptimizedSettingWindow : LocalizationEditorWindow
    {
        [MenuItem("Icarus/UGUI/Optimized Setting", false, 32)]
        static void Open()
        {
            var window = GetWindow<OptimizedSettingWindow>(true, "Optimized Setting", true);
        }


        public const string ToRectKey = "CloseToRectMask";
        
        public const string CheckHelper = "CheckHelper";

        private string[] _checkHelpers;
        protected override void On_Enable()
        {
            LoadCsvLanguageConfig(
                PathUtil.GetDataPathCombinePath(ConstTable.LocalizationBaseFolder, "OptimizedSettingWindow"), 1);

            _checkHelpers = TypeUtil.GetFilterSystemAssemblyQualifiedNames(typeof(ICheckHelper));

            _initCheckHelper();
        }

        private void _initCheckHelper()
        {
            var currentHelper = GetValue<string>(CheckHelper);
            
            if (string.IsNullOrEmpty(currentHelper))
            {
                if (_checkHelpers.Length != 0)
                {
                    SetValue(CheckHelper, _checkHelpers[_selectIndex]);
                }
                else
                {
                    SetValue(CheckHelper, string.Empty);
                    EditorFrameLog.Warning(GetLanguageValue("NoCheckHelper"));
                }
            }
            else
            {
                //初始化索引器
                for (var i = 0; i < _checkHelpers.Length; i++)
                {
                    if (currentHelper == _checkHelpers[i])
                    {
                        _selectIndex = i;
                    }
                }
            }
        }


        #region GUI

        private void OnGUI()
        {
            DrawLocalizationSelect();

            EditorGUILayout.Space();
            EditorGUILayoutUtil.DrawUILine(Color.white, width: position.size.x);
            EditorGUILayout.Space();

            _drawCheckHelper();
            
            EditorGUILayout.Space();
            EditorGUILayoutUtil.DrawUILine(Color.white, width: position.size.x);
            EditorGUILayout.Space();

            _drawMaskToRectMask();
        }

        private int _selectIndex;
        private void _drawCheckHelper()
        {
            EditorGUI.BeginChangeCheck();
            {
                _selectIndex = EditorGUILayout.Popup(new GUIContent(GetLanguageValue(CheckHelper),String.Format(GetLanguageValue($"{CheckHelper}Tooltip"),typeof(ICheckHelper))), _selectIndex,
                    _checkHelpers.Select(x => x.Split(',')[0]).ToArray());
            }
            if (EditorGUI.EndChangeCheck())
            {
                SetValue(CheckHelper,_checkHelpers[_selectIndex]);
            }
        }

        private void _drawMaskToRectMask()
        {
            EditorGUI.BeginChangeCheck();

            bool close =GetValue<bool>(ToRectKey);

            close = EditorGUILayout.Toggle(
                new GUIContent(GetLanguageValue(ToRectKey), GetLanguageValue($"{ToRectKey}Tooltip")), close);

            if (EditorGUI.EndChangeCheck())
            {
                SetValue(ToRectKey, close);
            }
        }

            #endregion

        T GetValue<T>(string key)
        {
            return Cfg.CSVEncrypting.GetValue<T>(GetKey(key));
        }

        void SetValue(string key, object value)
        {
            Cfg.CSVEncrypting.SetValue(GetKey(key), value);
        }

        public static string GetKey(string key)
        {
            return $"Optimized UGUI OptimizedSetting {key}";
        }
    }
}