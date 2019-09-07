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
        SelfAndOffspring,
        
        /// <summary>
        /// 跳过但是需要检查子孙
        /// </summary>
        ButCheckOffspring,
    }
    /// <summary>
    /// 验证器
    /// </summary>
    public interface ICheckHelper
    {
        /// <summary>
        /// 验证是否跳过,这个方法里你将写
        /// 一些人肉代码,简单而多的代码,但可以做很多事情,可以写自己的跳过策略来处理层级,暂时没完善,现在只会不跳过的都会在跳过的之后
        /// </summary>
        /// <param name="path">物体相对路径</param>
        /// <param name="obj">物体</param>
        /// <returns></returns>
        SkipType CheckKSkip(string path,RectTransform obj);

        /// <summary>
        /// 当物体路径被修改时触发
        /// </summary>
        /// <param name="oldPath">老路径</param>
        /// <param name="newPath">新路径</param>
        void OnChangePath(string oldPath, string newPath);

        /// <summary>
        /// Text 组件处理
        /// </summary>
        /// <param name="text"></param>
        void TextHandle(RectTransform text);
    }
}