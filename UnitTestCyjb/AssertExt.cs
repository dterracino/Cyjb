﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
    /// <summary>
    /// 表示 Assert 类的相关扩展方法。
    /// </summary>
    internal static class AssertExt
	{
		/// <summary>
		/// 验证两个集合是否相同。如果不同，则断言失败。
		/// </summary>
		/// <param name="expected">期待出现的集合。</param>
		/// <param name="actual">实际得到的集合。</param>
		public static void SetEqual<T>(T[] expected, IEnumerable<T> actual)
		{
			if (expected == null)
			{
				if (actual != null)
				{
					Assert.Fail("实际的集合 {{{0}}} 不为 null", string.Join(", ", actual));
				}
			}
			else if (actual == null)
			{
				Assert.Fail("实际的集合为 null");
			}
			else
			{
				var expectedSet = new HashSet<T>(expected);
				var actualSet = new HashSet<T>(expected);
				if (!expectedSet.SetEquals(actualSet))
				{
					Assert.Fail("期望得到 {{{0}}}，而实际得到的是 {{{1}}}",
						string.Join(", ", expectedSet), string.Join(", ", actualSet));
				}
			}
		}
		/// <summary>
		/// 验证两个数组是否相同。如果不同，则断言失败。
		/// </summary>
		/// <param name="expected">期待出现的数组。</param>
		/// <param name="actual">实际得到的数组。</param>
		public static void AreEqual<T>(T[] expected, T[] actual)
		{
			if (expected == null)
			{
				if (actual != null)
				{
					Assert.Fail("实际的数组 {{{0}}} 不为 null", string.Join(", ", actual));
				}
			}
			else if (actual == null)
			{
				Assert.Fail("实际的数组为 null");
			}
			else
			{
				for (var i = 0; i < expected.Length; i++)
				{
					if (!EqualityComparer<T>.Default.Equals(expected[i], actual[i]))
					{
						Assert.Fail("期望得到 {{{0}}}，而实际得到的是 {{{1}}}",
							string.Join(", ", expected), string.Join(", ", actual));
					}
				}
			}
		}
		/// <summary>
		/// 验证方法是否抛出了指定的异常。如果未抛出异常或不在期待的类型中，
		/// 则断言失败。
		/// </summary>
		/// <param name="action">要测试的方法。</param>
		/// <param name="expectedException">期待抛出的异常集合。</param>
		public static void ThrowsException(Action action, params Type[] expectedException)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				// 检查是否是期望的异常。
				if (expectedException.Any(t => t.IsInstanceOfType(ex)))
				{
					return;
				}
				throw;
			}
			// 未发生异常。
			Assert.Fail("没有抛出期望的异常 {0}", string.Join(", ", expectedException.AsEnumerable()));
		}
	}
}
