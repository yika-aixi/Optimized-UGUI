using System;
using System.Collections.Generic;
using System.Linq;
using CabinIcarus.EditorFrame.Base.Editor;
using CabinIcarus.EditorFrame.Config;
using CabinIcarus.EditorFrame.Log;
using CabinIcarus.EditorFrame.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        
        public const string IncludeInactive = "IncludeInactive";

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

            _drawToggle(ToRectKey);
            _drawToggle(IncludeInactive);
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

        private void _drawToggle(string key)
        {
            EditorGUI.BeginChangeCheck();

            bool close = GetValue<bool>(key);

            close = EditorGUILayout.Toggle(
                new GUIContent(GetLanguageValue(key), GetLanguageValue($"{key}Tooltip")), close);

            if (EditorGUI.EndChangeCheck())
            {
                SetValue(key, close);
            }
        }

        #endregion

        #region Command 

        private static List<RectTransform> _notTextUI = new List<RectTransform>();
        private static List<RectTransform> _textUI = new List<RectTransform>();

        [MenuItem("GameObject/Icarus/Optimized", false, -99999)]
        static void Optimized()
        {
            _clearList();
            var ui = Selection.activeTransform;

            var checkHelper = (ICheckHelper) Activator.CreateInstance(Type.GetType(GetValue<string>(CheckHelper)));

            _childHandle(ui,ui,checkHelper);

            Undo.RecordObject(ui,$"Optimized {ui.name}");

            GameObject go = null;
            
            foreach (var notText in _notTextUI)
            {
                go = _getNewParent(notText,ui);

                if (!go)
                {
                    continue;
                }
                
                Undo.SetTransformParent(notText.transform,go.transform,"Set Parent");
                notText.SetParent(go.transform,false);
            }
            
            foreach (var text in _textUI)
            {
                go = _getNewParent(text,ui);
                
                if (!go)
                {
                    continue;
                }
                
                Undo.SetTransformParent(text.transform,go.transform,"Set Parent");
                text.SetParent(go.transform,false);
                checkHelper.TextHandle(text);
            }
            
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void _clearList()
        {
            _notTextUI.Clear();
            _textUI.Clear();
        }

        private static GameObject _getNewParent(RectTransform notText,Transform root)
        {
            //存在父级,同时不是Root
            if (notText.parent && notText.parent != root)
            {
                //克隆一个一样设置的父级
                //保证ui不出问题
                var parent = notText.parent;
                var  newParent = new GameObject(parent.name);
                newParent.transform.SetParent(root,false);
                newParent.AddComponent<RectTransform>().CopyTarget(parent as RectTransform);
                Undo.RegisterCreatedObjectUndo(newParent,"Create New Parent");
                return newParent;
            }

            return null;
        }

        /// <summary>
        /// 子孙处理
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="currentChild">当前子孙</param>
        /// <param name="checkHelper">检查器</param>
        /// <param name="isCheckOffspring">是否是检查子孙</param>
        private static void _childHandle(Transform root, Transform currentChild, ICheckHelper checkHelper,bool isCheckOffspring = false)
        {
            for (var i = 0; i < currentChild.childCount; i++)
            {
                var child = currentChild.GetChild(i);
                
                //隐藏物体
                if (!child.gameObject.activeSelf)
                {
                    //如果开启检查
                    if (!GetValue<bool>(IncludeInactive))
                    {
                        return;
                    }
                }

                var path = GameobjectUtil.GetPath(root.gameObject, child.gameObject);
                
                SkipType skipType;
                
                if (!isCheckOffspring)
                {
                    skipType  = checkHelper.CheckKSkip(path,
                        child as RectTransform);
                }
                else
                {
                    skipType = checkHelper.CheckOffspring(path, (RectTransform) root, (RectTransform) child);
                }

                switch (skipType)
                {
                    case SkipType.None:
                    case SkipType.Self:
                        if (skipType == SkipType.None)
                        {
                            _addList(child);
                        }

                        _childHandle(root, child, checkHelper);
                        
                        break;
                    case SkipType.SelfAndOffspring:
                        break;
                    case SkipType.ButCheckOffspring:
                        _childHandle(child,child,checkHelper,true);
                        break;
                }
            }
        }

        private static void _addList(Transform child)
        {
            //todo 不做类型判断,直接强转,因为是ui,如果出现Transform的情况那是ui有问题,不给予处理
            
            if (child.GetComponent<Text>())
            {
                _textUI.Add((RectTransform) child);
            }
            else
            {
                _notTextUI.Add((RectTransform) child);
            }
        }


        [MenuItem("GameObject/Icarus/Optimized", true)]
        static bool OptimizedValidate()
        {
            var obj = Selection.activeTransform;
            
            return obj && obj is RectTransform && !string.IsNullOrEmpty(GetValue<string>(CheckHelper));
        }
        
        

        #endregion

        static T GetValue<T>(string key)
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