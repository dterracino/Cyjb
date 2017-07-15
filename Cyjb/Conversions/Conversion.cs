﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示类型转换的 IL 指令。
	/// </summary>
	internal abstract class Conversion
	{
		/// <summary>
		/// 当前转换的类型。
		/// </summary>
		public readonly ConversionType ConversionType;
		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="Conversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		internal Conversion(ConversionType conversionType)
		{
			ConversionType = conversionType;
		}
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public virtual void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			Contract.Requires(generator != null);
			Contract.Requires(inputType != null && outputType != null);
		}
	}
}