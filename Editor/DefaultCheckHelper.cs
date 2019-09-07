//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年09月07日-16:08
//Assembly-CSharp-Editor

using UnityEngine;

namespace CabinIcarus.OptimizedUGUI
{
    public class DefaultCheckHelper:ICheckHelper
    {
        public virtual SkipType CheckKSkip(string path, GameObject obj)
        {
            // 存在动画,跳过自身和子孙
            if (obj.GetComponent<Animator>())
            {
                return SkipType.SelfAndOffspring;
            }

            return SkipType.None;
        }

        public virtual void OnChangePath(string oldPath, string newPath)
        {
        }
    }
}