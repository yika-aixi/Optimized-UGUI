//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年09月07日-16:08
//Assembly-CSharp-Editor

using UnityEngine;
using UnityEngine.UI;

namespace CabinIcarus.OptimizedUGUI
{
    public class DefaultCheckHelper:ICheckHelper
    {
        public virtual SkipType CheckKSkip(string path, RectTransform obj)
        {
            // 存在动画,跳过自身和子孙
            if (obj.GetComponent<Animator>())
            {
                return SkipType.SelfAndOffspring;
            }
            
            // 下拉,跳过但是需要检查子孙,后面
//            if (obj.GetComponent<Dropdown>())
//            {
//                return SkipType.ButCheckOffspring;
//            }

            return SkipType.None;
        }

        public virtual void OnChangePath(string oldPath, string newPath)
        {
        }

        /// <summary>
        /// Text组件默认将Z轴调整到1,最后绘制
        /// </summary>
        /// <param name="text"></param>
        public void TextHandle(RectTransform text)
        {
            var anchoredPosition3D = text.anchoredPosition3D;
            
            anchoredPosition3D = new Vector3(anchoredPosition3D.x,anchoredPosition3D.y, 1);
            
            text.anchoredPosition3D = anchoredPosition3D;
        }
    }
}