using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Cyjb.Collections;

namespace Cyjb.Reflection
{
    /// <summary>
    /// 方法的实参列表信息。
    /// </summary>
    internal class MethodArgumentsInfo
	{
		/// <summary>
		/// 返回方法的参数信息。
		/// </summary>
		/// <param name="method">要获取参数信息的方法。</param>
		/// <param name="types">方法实参类型数组，其长度必须大于等于方法的参数个数。
		/// 使用 <see cref="Missing"/> 表示无需进行类型检查，<c>null</c> 表示引用类型标志。</param>
		/// <param name="options">方法参数信息的选项。</param>
		/// <returns>方法的参数信息。</returns>
		public static MethodArgumentsInfo GetInfo(MethodBase method, Type[] types, MethodArgumentsOption options)
		{
			Contract.Requires(method != null && types != null);
			var result = new MethodArgumentsInfo(method, types);
			var optionalParamBinding = options.HasFlag(MethodArgumentsOption.OptionalParamBinding);
			var isExplicit = options.HasFlag(MethodArgumentsOption.Explicit);
			var convertRefType = options.HasFlag(MethodArgumentsOption.ConvertRefType);
			// 填充方法实例。
			var offset = 0;
			if (options.HasFlag(MethodArgumentsOption.ContainsInstance))
			{
				if (!result.MarkInstanceType(isExplicit, convertRefType))
				{
					return null;
				}
				offset++;
			}
			// 填充 params 参数和可变参数。
			if (!result.FillParamArray(isExplicit, convertRefType))
			{
				return null;
			}
			// 检查实参是否与形参对应，未对应的参数是否包含默认值。
			var parameters = method.GetParametersNoCopy();
			var paramLen = parameters.Length;
			if (result.ParamArrayType != null)
			{
				paramLen--;
				Contract.Assume(paramLen >= 0);
			}
			for (int i = 0, j = offset; i < paramLen; i++, j++)
			{
				if (!CheckParameter(parameters[i], types[j], optionalParamBinding, isExplicit, convertRefType))
				{
					return null;
				}
			}
			result.fixedArguments = new ArrayAdapter<Type>(types, offset, paramLen);
			return result;
		}
		/// <summary>
		/// 方法信息。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MethodBase method;
		/// <summary>
		/// 方法实参类型列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type[] arguments;
		/// <summary>
		/// 方法实例实参类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type instanceType;
		/// <summary>
		/// 方法的固定实参列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ArrayAdapter<Type> fixedArguments;
		/// <summary>
		/// params 形参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type paramArrayType;
		/// <summary>
		/// params 实参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Type> paramArgumentTypes;
		/// <summary>
		/// 可变参数的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Type> optionalArgumentTypes;
		/// <summary>
		/// 方法泛型参数类型的推断结果。
		/// </summary>
		public Type[] GenericArguments;
		/// <summary>
		/// 使用指定的方法信息和实参类型列表初始化 <see cref="MethodArgumentsInfo"/> 类的新实例。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <param name="types">方法实参类型列表。</param>
		private MethodArgumentsInfo(MethodBase method, Type[] types)
		{
			Contract.Requires(method != null && types != null);
			this.method = method;
			arguments = types;
		}
		/// <summary>
		/// 获取方法实例实参类型。
		/// </summary>
		/// <value>方法实例实参类型。<c>null</c> 表示不是实例方法。</value>
		public Type InstanceType
		{
			get { return instanceType; }
		}
		/// <summary>
		/// 获取方法的固定实参列表。
		/// </summary>
		/// <value>方法的固定实参列表。如果 <see cref="ParamArrayType"/> 不为 <c>null</c>，
		/// 则不包含最后的 params 参数。</value>
		/// <remarks>列表元素为 <see cref="Missing"/> 表示使用参数默认值或空数组（对于 params 参数）；
		/// 为 <c>null</c> 表示实参值是 <c>null</c>，仅具有引用类型的约束。</remarks>
		public IList<Type> FixedArguments
		{
			get { return fixedArguments; }
		}
		/// <summary>
		/// 获取 params 形参的类型。
		/// </summary>
		/// <value>params 形参的类型，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		public Type ParamArrayType
		{
			get { return paramArrayType; }
		}
		/// <summary>
		/// 获取 params 实参的类型列表。
		/// </summary>
		/// <value>params 实参的类型列表，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		/// <remarks>列表元素为 <c>null</c> 表示实参值是 <c>null</c>，仅具有引用类型的约束。</remarks>
		public IList<Type> ParamArgumentTypes
		{
			get { return paramArgumentTypes; }
		}
		/// <summary>
		/// 获取可变参数的类型。
		/// </summary>
		/// <value>可变参数的类型，如果为 <c>null</c> 表示没有可变参数。</value>
		/// <remarks>列表元素为 <c>null</c> 表示实参值是 <c>null</c>，仅具有引用类型的约束。</remarks>
		public IList<Type> OptionalArgumentTypes
		{
			get { return optionalArgumentTypes; }
		}
		/// <summary>
		/// 清除 params 参数信息，表示无需特殊处理该参数。
		/// </summary>
		public void ClearParamArrayType()
		{
			if (paramArrayType != null)
			{
				paramArrayType = null;
				paramArgumentTypes = null;
				fixedArguments = new ArrayAdapter<Type>(arguments, fixedArguments.Offset,
					fixedArguments.Count + 1);
			}
		}
		/// <summary>
		/// 更新 params 参数信息，因为该参数类型中可能包含泛型类型参数。
		/// </summary>
		/// <param name="newMethod">方法信息。</param>
		public void UpdateParamArrayType(MethodBase newMethod)
		{
			Contract.Requires(newMethod != null);
			if (paramArrayType != null)
			{
				var parameters = newMethod.GetParametersNoCopy();
				paramArrayType = parameters[parameters.Length - 1].ParameterType;
			}
		}

		#region 获取方法参数类型

		/// <summary>
		/// 将实参中的第一个参数作为方法的实例。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果第一个参数可以作为方法的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool MarkInstanceType(bool isExplicit, bool convertRefType)
		{
			if (method.IsStatic)
			{
				return false;
			}
			if (arguments.Length == 0)
			{
				return false;
			}
			instanceType = arguments[0];
			if (instanceType == null)
			{
				instanceType = method.DeclaringType;
				Contract.Assume(instanceType != null);
				return !instanceType.IsValueType;
			}
			if (instanceType == typeof(Missing))
			{
				return true;
			}
			if (instanceType.IsByRef)
			{
				instanceType = instanceType.GetElementType();
				if (!convertRefType)
				{
					return method.DeclaringType == instanceType;
				}
			}
			var declaringType = method.DeclaringType;
			Contract.Assume(declaringType != null);
			return declaringType.IsConvertFrom(instanceType, isExplicit);
		}
		/// <summary>
		/// 填充 params 参数和可变参数。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果第一个参数可以作为方法的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <returns>如果填充参数成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool FillParamArray(bool isExplicit, bool convertRefType)
		{
			var parameters = method.GetParametersNoCopy();
			var paramLen = parameters.Length;
			var offset = instanceType == null ? 0 : 1;
			offset += paramLen;
			if (method.CallingConvention.HasFlag(CallingConventions.VarArgs))
			{
				optionalArgumentTypes = new ArrayAdapter<Type>(arguments, offset);
				return optionalArgumentTypes.All(type => type != null);
			}
			if (paramLen > 0)
			{
				var lastParam = parameters[paramLen - 1];
				if (lastParam.IsParamArray())
				{
					paramArrayType = lastParam.ParameterType;
					paramArgumentTypes = new ArrayAdapter<Type>(arguments, offset - 1);
					return lastParam.ParameterType.ContainsGenericParameters || CheckParamArrayType(isExplicit, convertRefType);
				}
			}
			// 检测方法实参数量。
			return arguments.Length <= offset;
		}
		/// <summary>
		/// 检查 params 参数类型。
		/// </summary>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果 params 参数类型匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParamArrayType(bool isExplicit, bool convertRefType)
		{
			var paramCnt = paramArgumentTypes.Count;
			if (paramCnt == 0)
			{
				return true;
			}
			var paramElementType = paramArrayType.GetElementType();
			if (paramCnt == 1)
			{
				// 只有一个实参，可能是数组或数组元素。
				var type = paramArgumentTypes[0];
				bool isTypeMatch;
				if (type == null || type == typeof(Missing))
				{
					isTypeMatch = true;
				}
				else
				{
					if (type.IsByRef)
					{
						type = type.GetElementType();
						if (!convertRefType)
						{
							return false;
						}
					}
					isTypeMatch = paramArrayType.IsConvertFrom(type, isExplicit);
				}
				if (isTypeMatch)
				{
					// 实参是数组，无需进行特殊处理。
					paramArrayType = null;
					paramArgumentTypes = null;
					return true;
				}
				return paramElementType.IsConvertFrom(type, isExplicit);
			}
			// 有多个实参，必须是数组元素。
			for (var i = 0; i < paramCnt; i++)
			{
				var type = paramArgumentTypes[i];
				if (type == null)
				{
					if (paramElementType.IsValueType)
					{
						return false;
					}
				}
				else if (type.IsByRef)
				{
					if (convertRefType)
					{
						if (!paramElementType.IsConvertFrom(type, isExplicit))
						{
							return false;
						}
					}
					else if (paramElementType != type)
					{
						return false;
					}
				}
				else if (type == typeof(Missing) || !paramElementType.IsConvertFrom(type, isExplicit))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 检查方法参数。
		/// </summary>
		/// <param name="parameter">要检查的方法参数。</param>
		/// <param name="type">方法实参类型。</param>
		/// <param name="optionalParamBinding">是否对可选参数进行绑定。</param>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果方法参数与实参兼容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckParameter(ParameterInfo parameter, Type type, bool optionalParamBinding,
			bool isExplicit, bool convertRefType)
		{
			var paramType = parameter.ParameterType;
			if (paramType.ContainsGenericParameters)
			{
				return true;
			}
			if (type == typeof(Missing))
			{
				// 检查可选参数和 params 参数。
				return parameter.IsParamArray() || (optionalParamBinding && parameter.HasDefaultValue);
			}
			var isByRef = false;
			if (paramType.IsByRef)
			{
				paramType = paramType.GetElementType();
				isByRef = true;
			}
			if (type == null)
			{
				if (isByRef && !convertRefType)
				{
					return false;
				}
				// 检查引用类型。
				return !paramType.IsValueType;
			}
			if (type.IsByRef)
			{
				if (isByRef)
				{
					return type.GetElementType() == paramType;
				}
				if (!convertRefType)
				{
					return false;
				}
				type = type.GetElementType();
			}
			return paramType.IsConvertFrom(type, isExplicit);
		}

		#endregion // 获取方法参数类型

	}
	/// <summary>
	/// 方法实参的选项。
	/// </summary>
	[Flags]
	public enum MethodArgumentsOption
	{
		/// <summary>
		/// 无任何选项。
		/// </summary>
		None = 0,
		/// <summary>
		/// 方法的第一个实参表示方法实例。
		/// </summary>
		ContainsInstance = 1,
		/// <summary>
		/// 对可选参数进行绑定。
		/// </summary>
		OptionalParamBinding = 2,
		/// <summary>
		/// 类型检查时，使用显式类型转换，而不是默认的隐式类型转换。
		/// </summary>
		Explicit = 4,
		/// <summary>
		/// 允许对按引用传递的类型进行类型转换。
		/// </summary>
		ConvertRefType = 8,
		/// <summary>
		/// 对可选参数进行绑定，且使用显式类型转换。
		/// </summary>
		OptionalAndExplicit = 6
	}
}