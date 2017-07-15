﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyjb
{
	/// <summary>
	/// 指定属性或事件的说明。
	/// </summary>
	internal sealed class ResDescriptionAttribute : DescriptionAttribute
	{
		/// <summary>
		/// 说明是否已本地化。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool _localized;

		/// <summary>
		/// 初始化 <see cref="Cyjb.ResDescriptionAttribute"/> 类的新实例。
		/// </summary>
		public ResDescriptionAttribute() { }

		/// <summary>
		/// 初始化 <see cref="Cyjb.ResDescriptionAttribute"/>
		/// 类的新实例并带有说明。
		/// </summary>
		/// <param name="description">说明文本。</param>
		public ResDescriptionAttribute(string description) : base(description) { }

		/// <summary>
		/// 获取存储在此特性中的说明。
		/// </summary>
		/// <value>存储在此特性中的说明。</value>
		public override string Description
		{
			get
			{
			    if (!_localized)
			    {
			        DescriptionValue = Resources.ResourceManager.GetString(DescriptionValue, Resources.Culture);
			        _localized = true;
			    }
			    return DescriptionValue;
			}
		}
	}
}
