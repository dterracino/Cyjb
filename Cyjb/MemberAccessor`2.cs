﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示实例属性或字段的存取器。
	/// </summary>
	/// <typeparam name="TTarget">包含实例属性或字段的对象的类型。</typeparam>
	/// <typeparam name="TValue">实例属性或字段值的类型。</typeparam>
	public sealed class MemberAccessor<TTarget, TValue>
	{
		/// <summary>
		/// 实例属性或字段的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;
		/// <summary>
		/// 获取实例属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Func<TTarget, TValue> getDelegate;
		/// <summary>
		/// 设置实例属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<TTarget, TValue> setDelegate;

		#region 从委托创建

		/// <summary>
		/// 使用指定的名字和访问委托，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="name">实例属性或字段的名字。</param>
		/// <param name="getDelegate">用于获取实例属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置实例属性或字段的委托。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><paramref name="getDelegate"/> 和 <paramref name="setDelegate"/> 
		/// 全部为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public MemberAccessor(string name, Func<TTarget, TValue> getDelegate, Action<TTarget, TValue> setDelegate)
		{
			CommonExceptions.CheckStringEmpty(name, nameof(name));
			if (getDelegate == null && setDelegate == null)
			{
				throw CommonExceptions.ArgumentBothNull(nameof(getDelegate), nameof(setDelegate));
			}
			Contract.EndContractBlock();
			this.name = name;
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
		}

		#endregion // 从委托创建

		#region 从类型创建

		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(string name)
		{
			CommonExceptions.CheckStringEmpty(name, nameof(name));
			Contract.EndContractBlock();
			this.name = name;
			Init(typeof(TTarget), false);
		}
		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(string name, bool nonPublic)
		{
			CommonExceptions.CheckStringEmpty(name, nameof(name));
			Contract.EndContractBlock();
			this.name = name;
			Init(typeof(TTarget), nonPublic);
		}
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name)
		{
			CommonExceptions.CheckArgumentNull(targetType, nameof(targetType));
			CommonExceptions.CheckStringEmpty(name, nameof(name));
			Contract.EndContractBlock();
			this.name = name;
			Init(targetType, false);
		}
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(targetType, nameof(targetType));
			CommonExceptions.CheckStringEmpty(name, nameof(name));
			Contract.EndContractBlock();
			this.name = name;
			Init(targetType, nonPublic);
		}
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(property, nameof(property));
			Contract.EndContractBlock();
			name = property.Name;
			Init(property, false);
		}
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(property, nameof(property));
			Contract.EndContractBlock();
			name = property.Name;
			Init(property, nonPublic);
		}
		/// <summary>
		/// 使用指定的字段，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的字段。
		/// </summary>
		/// <param name="field">要访问的字段。</param>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public MemberAccessor(FieldInfo field)
		{
			CommonExceptions.CheckArgumentNull(field, nameof(field));
			Contract.EndContractBlock();
			name = field.Name;
			Init(field);
		}

		#endregion // 从类型创建

		#region 初始化

		/// <summary>
		/// 使用指定的搜索类型初始化当前实例。
		/// </summary>
		/// <param name="type">要搜索的类型。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(Type type, bool nonPublic)
		{
			Contract.Requires(type != null);
			var flags = nonPublic ? TypeExt.AllMemberFlag : TypeExt.PublicFlag;
			var property = type.GetProperty(name, flags);
			if (property != null)
			{
				Init(property, nonPublic);
				return;
			}
			var field = type.GetField(name, flags);
			if (field != null)
			{
				Init(field);
			}
			throw CommonExceptions.PropertyOrFieldNotFound(name, nonPublic);
		}
		/// <summary>
		/// 使用指定的实例属性初始化当前实例。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(PropertyInfo property, bool nonPublic)
		{
			Contract.Requires(property != null);
			var method = property.GetGetMethod(nonPublic);
			if (method != null)
			{
				getDelegate = method.CreateDelegate<Func<TTarget, TValue>>();
			}
			method = property.GetSetMethod(nonPublic);
			if (method != null)
			{
				setDelegate = method.CreateDelegate<Action<TTarget, TValue>>();
			}
		}
		/// <summary>
		/// 使用指定的字段初始化当前实例。
		/// </summary>
		/// <param name="field">要访问的字段。</param>
		private void Init(FieldInfo field)
		{
			Contract.Requires(field != null);
			getDelegate = field.CreateDelegate<Func<TTarget, TValue>>(false);
			setDelegate = field.CreateDelegate<Action<TTarget, TValue>>(false);
		}

		#endregion // 初始化

		/// <summary>
		/// 获取实例属性或字段的名称。
		/// </summary>
		/// <value>实例属性或字段的名称。</value>
		public string Name
		{
			get
			{
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return name;
			}
		}
		/// <summary>
		/// 获取指定对象的实例属性或字段的值。
		/// </summary>
		/// <param name="target">要获取实例属性或字段值的对象。</param>
		/// <returns>指定对象的实例属性或字段的值。</returns>
		public TValue GetValue(TTarget target)
		{
			if (getDelegate == null)
			{
				throw CommonExceptions.PropertyNoGetter(name);
			}
			return getDelegate(target);
		}
		/// <summary>
		/// 设置指定对象的实例属性或字段的值。
		/// </summary>
		/// <param name="target">要获取实例属性或字段值的对象。</param>
		/// <param name="value">指定对象的实例属性或字段的值。</param>
		public void SetValue(TTarget target, TValue value)
		{
			if (setDelegate == null)
			{
				throw CommonExceptions.PropertyNoSetter(name);
			}
			setDelegate(target, value);
		}
	}
}
