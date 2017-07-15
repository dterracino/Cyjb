﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyjb.Configurations;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示缓冲池配置元素的集合。
	/// </summary>
	public sealed class CacheElementCollection : ConfigurationElementCollection<string, CacheElement>
	{
	    /// <summary>
		/// 创建一个新的 <see cref="ConfigurationElement"/>。
		/// </summary>
		/// <returns>新的 <see cref="ConfigurationElement"/>。</returns>
		/// <overloads>
		/// <summary>
		/// 创建一个新的 <see cref="ConfigurationElement"/>。
		/// </summary>
		/// </overloads>
		protected override ConfigurationElement CreateNewElement()
		{
			return new CacheElement();
		}
		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 <see cref="ConfigurationElement"/>。</param>
		/// <returns>一个 <see cref="Object"/>，用作指定 <see cref="ConfigurationElement"/> 的键。
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CacheElement)element).Key;
		}
	}
}
