//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年09月07日-15:34
//Assembly-CSharp-Editor

using UnityEngine;

namespace CabinIcarus.OptimizedUGUI
{
    public enum SkipType
    {
        /// <summary>
        /// 不跳过
        /// </summary>
        None,
        
        /// <summary>
        /// 自身
        /// </summary>
        Self,
        
        /// <summary>
        /// 自身和子孙
        /// </summary>
        SelfAndOffspring
    }
    /// <summary>
    /// 验证器
    /// </summary>
    public interface ICheckHelper
    {
        /// <summary>
        /// 验证是否跳过
        /// </summary>
        /// <param name="path">物体相对路径</param>
        /// <param name="obj">物体</param>
        /// <returns></returns>
        SkipType CheckKSkip(string path,GameObject obj);

        /// <summary>
        /// 当物体路径被修改时触发
        /// </summary>
        /// <param name="oldPath">老路径</param>
        /// <param name="newPath">新路径</param>
        void OnChangePath(string oldPath, string newPath);
    }
}