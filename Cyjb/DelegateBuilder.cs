using System;
using Cyjb.Reflection;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Cyjb
{
	/// <summary>
	/// 表示通用方法的调用委托。
	/// </summary>
	/// <param name="instance">要调用方法的实例。</param>
	/// <param name="parameters">方法的参数。</param>
	/// <returns>方法的返回值。</returns>
	public delegate object MethodInvoker(object instance, params object[] parameters);
	/// <summary>
	/// 表示通用构造函数的调用委托。
	/// </summary>
	/// <param name="parameters">构造函数的参数。</param>
	/// <returns>新创建的实例。</returns>
	public delegate object InstanceCreator(params object[] parameters);
	/// <summary>
	/// 提供动态构造方法、属性或字段委托的方法。
	/// </summary>
	/// <example>
	/// 一下是一些简单的示例，很容易构造出需要的委托。
	/// <code>
	/// class Program {
	/// 	public delegate void MyDelegate(params int[] args);
	/// 	public static void TestMethod(int value) { }
	/// 	public void TestMethod(uint value) { }
	/// 	public static void TestMethod{T}(params T[] arg) { }
	/// 	static void Main(string[] args) {
	/// 		Type type = typeof(Program);
	/// 		Action&lt;int&gt; m1 = type.CreateDelegate&lt;Action&lt;int&gt;&gt;("TestMethod");
	/// 		m1(10);
	/// 		Program p = new Program();
	/// 		Action&lt;Program, uint&gt; m2 = type.CreateDelegate&lt;Action&lt;Program, uint&gt;&gt;("TestMethod");
	/// 		m2(p, 10);
	/// 		Action&lt;object, uint} m3 = type.CreateDelegate&lt;Action&lt;object, uint&gt;&gt;("TestMethod");
	/// 		m3(p, 10);
	/// 		Action&lt;uint} m4 = type.CreateDelegate&lt;Action&lt;uint&gt;&gt;("TestMethod", p);
	/// 		m4(10);
	/// 		MyDelegate m5 = type.CreateDelegate&lt;MyDelegate&gt;("TestMethod");
	/// 		m5(0, 1, 2);
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// <para><see cref="DelegateBuilder"/> 类提供的 <c>CreateDelegate</c> 方法，其的用法与 
	/// <c>Delegate.CreateDelegate</c> 完全相同，功能却大大丰富，
	/// 几乎可以只依靠委托类型、反射类型和成员名称构造出任何需要的委托，
	/// 省去了自己反射获取类型成员的过程。</para>
	/// <para>关于对反射创建委托的效率问题，以及该类的实现原理，可以参见我的博文 
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/DelegateBuilder.html">
	/// 《C# 反射的委托创建器》</see>。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/DelegateBuilder.html">《C# 反射的委托创建器》</seealso>
	public static partial class DelegateBuilder
	{

		#region 通用委托

		/// <summary>
		/// 创建表示指定的静态或实例方法的的委托。如果是实例方法，需要将实例对象作为第一个参数；
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <returns>表示指定的静态或实例方法的委托。</returns>
		/// <remarks>如果是静态方法，则第一个参数无效。对于可变参数方法，只支持固定参数。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="method"/> 是开放构造方法。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static MethodInvoker CreateDelegate(this MethodInfo method)
		{
			CommonExceptions.CheckArgumentNull(method, nameof(method));
			Contract.Ensures(Contract.Result<MethodInvoker>() != null);
			if (method.ContainsGenericParameters)
			{
				// 不能对开放构造方法执行绑定。
				throw CommonExceptions.BindOpenConstructedMethod(nameof(method));
			}
			var dlgMethod = new DynamicMethod("MethodInvoker", typeof(object),
				new[] { typeof(object), typeof(object[]) }, method.Module, true);
			var il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			var parameters = method.GetParametersNoCopy();
			var len = parameters.Length;
			// 参数数量检测。
			if (len > 0)
			{
				il.EmitCheckArgumentNull(1, "parameters");
				il.Emit(OpCodes.Ldarg_1);
				il.EmitCheckTargetParameterCount(parameters.Length);
			}
			// 加载实例对象。
			if (!method.IsStatic)
			{
				il.EmitLoadInstance(method, typeof(object), true);
			}
			var optimizeTailcall = true;
			// 加载方法参数。
			for (var i = 0; i < len; i++)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.EmitInt(i);
				var paramType = parameters[i].ParameterType;
				if (paramType.IsByRef)
				{
					paramType = paramType.GetElementType();
					var converter = il.GetConversion(typeof(object), paramType, ConversionType.Explicit);
					Console.WriteLine(converter);
					if (converter.NeedEmit)
					{
						il.Emit(OpCodes.Ldelem_Ref);
						converter.Emit(true);
						var local = il.DeclareLocal(paramType);
						il.Emit(OpCodes.Stloc, local);
						il.Emit(OpCodes.Ldloca, local);
						optimizeTailcall = false;
					}
					else
					{
						il.Emit(OpCodes.Ldelema, paramType);
					}
				}
				else
				{
					il.Emit(OpCodes.Ldelem_Ref);
					il.EmitConversion(typeof(object), paramType, true, ConversionType.Explicit);
				}
			}
			// 调用函数。
			il.EmitInvokeMethod(method, null, typeof(object), optimizeTailcall);
			return (MethodInvoker)dlgMethod.CreateDelegate(typeof(MethodInvoker));
		}
		/// <summary>
		/// 创建表示指定的构造函数的的委托。
		/// </summary>
		/// <param name="ctor">描述委托要表示的构造函数的 <see cref="ConstructorInfo"/>。</param>
		/// <returns>表示指定的构造函数的委托。</returns>
		/// <remarks>对于可变参数方法，只支持固定参数。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="ctor"/>。</exception>
		public static InstanceCreator CreateDelegate(this ConstructorInfo ctor)
		{
			CommonExceptions.CheckArgumentNull(ctor, nameof(ctor));
			Contract.EndContractBlock();
			var dlgMethod = new DynamicMethod("InstanceCreator", typeof(object),
				new[] { typeof(object[]) }, ctor.Module, true);
			var il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			var parameters = ctor.GetParametersNoCopy();
			var len = parameters.Length;
			// 参数数量检测。
			if (len > 0)
			{
				il.EmitCheckArgumentNull(0, "parameters");
				il.Emit(OpCodes.Ldarg_0);
				il.EmitCheckTargetParameterCount(parameters.Length);
			}
			// 加载方法参数。
			for (var i = 0; i < len; i++)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.EmitInt(i);
				il.Emit(OpCodes.Ldelem_Ref);
				il.EmitConversion(typeof(object), parameters[i].ParameterType, true, ConversionType.Explicit);
			}
			// 对实例进行类型转换。
			var converter = il.GetConversion(ctor.DeclaringType, typeof(object), ConversionType.Explicit);
			il.Emit(OpCodes.Newobj, ctor);
			converter.Emit(true);
			il.Emit(OpCodes.Ret);
			return (InstanceCreator)dlgMethod.CreateDelegate(typeof(InstanceCreator));
		}
		/// <summary>
		/// 创建表示指定类型的默认构造函数的的委托。
		/// </summary>
		/// <param name="type">描述委托要创建的实例的类型。</param>
		/// <returns>表示指定类型的默认构造函数的委托。</returns>
		/// <remarks>对于可变参数方法，只支持固定参数。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 包含泛型参数。</exception>
		public static InstanceCreator CreateInstanceCreator(this Type type)
		{
			CommonExceptions.CheckArgumentNull(type, nameof(type));
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(type);
			}
			var dlgMethod = new DynamicMethod("InstanceCreator", typeof(object),
				new[] { typeof(object[]) }, type.Module, true);
			var il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			// 对实例进行类型转换。
			var converter = il.GetConversion(type, typeof(object), ConversionType.Explicit);
			il.EmitNew(type);
			converter.Emit(true);
			il.Emit(OpCodes.Ret);
			return (InstanceCreator)dlgMethod.CreateDelegate(typeof(InstanceCreator));
		}

		#endregion // 通用委托

		#region 委托类型包装

		/// <summary>
		/// 将指定的委托用指定类型的委托包装，支持对参数进行强制类型转换。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="dlg">要被包装的委托。</param>
		/// <returns>指定类型的委托，其包装了 <paramref name="dlg"/>。
		/// 如果参数个数不同，或者参数间不能执行强制类型转换，则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		/// <overloads>
		/// <summary>
		/// 将指定的委托用指定类型的委托包装，支持对参数进行强制类型转换。
		/// </summary>
		/// </overloads>
		public static TDelegate Wrap<TDelegate>(this Delegate dlg)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(dlg, nameof(dlg));
			Contract.EndContractBlock();
			var typedDlg = dlg as TDelegate;
			if (typedDlg != null)
			{
				return typedDlg;
			}
			var type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			return CreateClosedDelegate(dlg.Method, type, dlg.Target, false) as TDelegate;
		}
		/// <summary>
		/// 将指定的委托用指定类型的委托包装，支持对参数进行强制类型转换。
		/// </summary>
		/// <param name="dlg">要被包装的委托。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>指定类型的委托，其包装了 <paramref name="dlg"/>。
		/// 如果参数个数不同，或者参数间不能执行强制类型转换，则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate Wrap(this Delegate dlg, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(dlg, nameof(dlg));
			CommonExceptions.CheckArgumentNull(delegateType, nameof(delegateType));
			Contract.EndContractBlock();
			if (delegateType.IsInstanceOfType(dlg))
			{
				return dlg;
			}
			CommonExceptions.CheckDelegateType(delegateType, nameof(delegateType));
			return CreateClosedDelegate(dlg.Method, delegateType, dlg.Target, false);
		}

		#endregion // 委托类型包装

	}
}