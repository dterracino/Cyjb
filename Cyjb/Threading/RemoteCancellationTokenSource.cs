﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cyjb.Threading
{
	/// <summary>
	/// 允许跨域通知 <see cref="CancellationToken"/>，告知其应被取消。
	/// </summary>
	/// <remarks>使用时请将 <see cref="RemoteCancellationTokenSource"/> 创建在新的 AppDomain 中，
	/// 并在原 AppDomain 中调用 <code>cancellationToken.Register(remoteCancellationTokenSource.Cancel);</code>，
	/// 将此 <see cref="RemoteCancellationTokenSource"/> 注册到原 AppDomain 的 <see cref="CancellationToken"/> 里。</remarks>
	[HostProtection(Synchronization = true, ExternalThreading = true)]
	[DebuggerDisplay("IsCancellationRequested = {IsCancellationRequested}")]
	public sealed class RemoteCancellationTokenSource : MarshalByRefObject, IDisposable
	{
		/// <summary>
		/// 内部的 <see cref="CancellationTokenSource"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CancellationTokenSource tokenSource;
		/// <summary>
		/// 初始化 <see cref="RemoteCancellationTokenSource"/> 类的新实例。
		/// </summary>
		public RemoteCancellationTokenSource()
		{
			tokenSource = new CancellationTokenSource();
		}
		/// <summary>
		/// 使用指定的延迟初始化 <see cref="RemoteCancellationTokenSource"/> 类的新实例。
		/// </summary>
		/// <param name="millisecondsDelay">取消  <see cref="RemoteCancellationTokenSource"/> 前的等待时间（毫秒）。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="RemoteCancellationTokenSource"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public RemoteCancellationTokenSource(int millisecondsDelay)
		{
			tokenSource = new CancellationTokenSource(millisecondsDelay);
		}
		/// <summary>
		/// 使用指定的时间间隔初始化 <see cref="RemoteCancellationTokenSource"/> 类的新实例。
		/// </summary>
		/// <param name="delay">取消  <see cref="RemoteCancellationTokenSource"/> 前的时间间隔。</param>
		public RemoteCancellationTokenSource(TimeSpan delay)
		{
			tokenSource = new CancellationTokenSource(delay);
		}
		/// <summary>
		/// 获取是否已请求取消此 <see cref="RemoteCancellationTokenSource"/>。
		/// </summary>
		/// <value>如果已请求取消此 <see cref="RemoteCancellationTokenSource"/>，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsCancellationRequested { get { return tokenSource.IsCancellationRequested; } }
		/// <summary>
		/// 获取与此 <see cref="RemoteCancellationTokenSource"/> 关联的 <see cref="CancellationToken"/>。
		/// </summary>
		/// <value>与此 <see cref="RemoteCancellationTokenSource"/> 关联的 <see cref="CancellationToken"/>。</value>
		public CancellationToken Token { get { return tokenSource.Token; } }

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public void Dispose()
		{
			tokenSource.Dispose();
		}

		#endregion // IDisposable 成员

		/// <summary>
		/// 传达取消请求。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 传达取消请求。
		/// </summary>
		/// </overloads>
		public void Cancel()
		{
			tokenSource.Cancel();
		}
		/// <summary>
		/// 传达对取消，并指定是否应处理其余回调和可取消操作。
		/// </summary>
		/// <param name="throwOnFirstException">如果异常应该直接传播，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public void Cancel(bool throwOnFirstException)
		{
			tokenSource.Cancel(throwOnFirstException);
		}
		/// <summary>
		/// 在指定等待时间后执行取消操作。
		/// </summary>
		/// <param name="millisecondsDelay">取消  <see cref="RemoteCancellationTokenSource"/> 前的等待时间（毫秒）。</param>
		/// <overloads>
		/// <summary>
		/// 在指定等待时间后执行取消操作。
		/// </summary>
		/// </overloads>
		public void CancelAfter(int millisecondsDelay)
		{
			tokenSource.CancelAfter(millisecondsDelay);
		}
		/// <summary>
		/// 在指定等待时间（毫秒）后执行取消操作。
		/// </summary>
		/// <param name="delay">取消  <see cref="RemoteCancellationTokenSource"/> 前的时间间隔。</param>
		public void CancelAfter(TimeSpan delay)
		{
			tokenSource.CancelAfter(delay);
		}
	}
}
