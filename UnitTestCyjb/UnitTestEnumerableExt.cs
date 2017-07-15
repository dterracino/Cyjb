﻿using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
    /// <summary>
    /// <see cref="EnumerableExt"/> 类的单元测试。
    /// </summary>
    [TestClass]
	public class UnitTestEnumerableExt
	{
		/// <summary>
		/// 对 <see cref="EnumerableExt.Iterative"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIterative()
		{
			AssertExt.AreEqual(new int[0], new int[0].Iterative().ToArray());
			AssertExt.AreEqual(new int[0], new[] { 0, 1, 2, 3 }.Iterative().ToArray());
			AssertExt.AreEqual(new[] { 0 }, new[] { 0, 0, 0, 0 }.Iterative().ToArray());
			AssertExt.AreEqual(new[] { 0, 1, 2 }, new[] { 0, 0, 1, 1, 2, 2 }.Iterative().ToArray());
			AssertExt.AreEqual(new[] { 1, 2, 0 }, new[] { 0, 1, 1, 2, 2, 3, 4, 5, 6, 0 }.Iterative().ToArray());
		}
	}
}
