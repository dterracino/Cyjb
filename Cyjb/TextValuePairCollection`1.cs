﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyjb
{
	/// <summary>
	/// 表示文本-值对的集合。
	/// </summary>
	/// <typeparam name="TValue">值的数据类型。</typeparam>
	[Serializable]
	public class TextValuePairCollection<TValue> :
		BindingList<TextValuePair<TValue>>
	{
	    /// <summary>
		/// 返回指定文本在集合中的索引。
		/// </summary>
		/// <param name="text">要获取索引的文本。</param>
		/// <returns>指定文本在集合中的索引，如果在集合中不存在，则返回 
		/// <c>-1</c>。 </returns>
		/// <overloads>
		/// <summary>
		/// 返回指定元素在集合中的索引。
		/// </summary>
		/// </overloads>
		public int IndexOf(string text)
		{
			for (var i = 0; i < Items.Count; i++)
			{
				if (string.Equals(Items[i].Text, text, StringComparison.CurrentCulture))
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// 将指定的文本和值添加到集合中。
		/// </summary>
		/// <param name="text">要添加的文本。</param>
		/// <param name="value">要添加的值。</param>
		/// <overloads>
		/// <summary>
		/// 将指定的文本和值添加到集合中。
		/// </summary>
		/// </overloads>
		public void Add(string text, TValue value)
		{
			Add(new TextValuePair<TValue>(text, value));
		}
		/// <summary>
		/// 获取一个值，该值指示列表是否支持搜索。
		/// </summary>
		/// <value>如果列表支持搜索，则为 <c>true</c>；否则为 <c>true</c>。
		/// </value>
		protected override bool SupportsSearchingCore
		{
			get { return true; }
		}
		/// <summary>
		/// 使用指定值搜索具有指定属性说明符的项的索引。
		/// </summary>
		/// <param name="prop">要搜索的 <see cref="PropertyDescriptor"/>。</param>
		/// <param name="key">要匹配的 <paramref name="prop"/> 值。</param>
		/// <returns>与属性说明符匹配并包含指定值的项的从零开始的索引。
		/// </returns>
		protected override int FindCore(PropertyDescriptor prop, object key)
		{
			if (prop != null && prop.Name == "Value")
			{
				if (key is TValue)
				{
					var value = (TValue)key;
					var comparer = EqualityComparer<TValue>.Default;
					for (var i = 0; i < Count; i++)
					{
						if (comparer.Equals(Items[i].Value, value))
						{
							return i;
						}
					}
				}
			}
			else
			{
				return IndexOf(key.ToString());
			}
			return -1;
		}
	}
}
