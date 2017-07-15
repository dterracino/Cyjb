using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.StringExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestStringExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.UnescapeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestUnescapeUnicode()
		{
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", "English or 中文 or \\u0061\\u0308 or \\uD834\\uDD60".UnescapeUnicode());
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", "English or 中文 or \\u0061\\u0308 or \\U0001D160".UnescapeUnicode());
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", "English or 中文 or \\u0061\\u0308 or \\uD834\\uDD60".UnescapeUnicode());
			Assert.AreEqual("\x25 \u0061\u0308 or \uD834\uDD60\\", "\x25 \\u0061\\u0308 or \\uD834\\uDD60\\".UnescapeUnicode());
			Assert.AreEqual("\x25\\x\x2\x25 \x25\x25 \u0061\u0308 or \uD834\uDD60\\", "\x25\\x\\x02\\x25 \\x25\\x25 \\u0061\\u0308 or \\uD834\\uDD60\\".UnescapeUnicode());
			Assert.AreEqual(null, StringExt.UnescapeUnicode(null));
			Assert.AreEqual("", "".UnescapeUnicode());
			Assert.AreEqual("\\", "\\".UnescapeUnicode());
			Assert.AreEqual("\\\\", "\\\\".UnescapeUnicode());
			Assert.AreEqual("\\\x1", "\\\\x01".UnescapeUnicode());
			Assert.AreEqual("\\\\\\", "\\\\\\".UnescapeUnicode());
			Assert.AreEqual("\\\\\x1", "\\\\\\x01".UnescapeUnicode());
			Assert.AreEqual("\\\\\\x1\\x2", "\\\\\\x1\\x2".UnescapeUnicode());
			Assert.AreEqual("\\ab", "\\ab".UnescapeUnicode());
			Assert.AreEqual("\\a\\b#556", "\\a\\b\\x23556".UnescapeUnicode());
			Assert.AreEqual("\\a\\b\u23556", "\\a\\b\\u23556".UnescapeUnicode());
			Assert.AreEqual("\\a\\b\\U23556", "\\a\\b\\U23556".UnescapeUnicode());
		}
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.EscapeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestEscapeUnicode()
		{
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", "English or 中文 or \u0061\u0308 or \uD834\uDD60".EscapeUnicode());
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", "English or 中文 or \u0061\u0308 or \U0001D160".EscapeUnicode());
			Assert.AreEqual("% a\\u0308 or \\uD834\\uDD60\\", "\x25 \u0061\u0308 or \uD834\uDD60\\".EscapeUnicode());
			Assert.AreEqual(null, StringExt.EscapeUnicode(null));
			Assert.AreEqual("", "".EscapeUnicode());
			Assert.AreEqual("\\", "\\".EscapeUnicode());
			Assert.AreEqual("\\\\", "\\\\".EscapeUnicode());
			Assert.AreEqual("\\\\u0001", "\\\x1".EscapeUnicode());
			Assert.AreEqual("\\\\\\", "\\\\\\".EscapeUnicode());
			Assert.AreEqual("\\\\\\u0001", "\\\\\x1".EscapeUnicode());
			Assert.AreEqual("\\ab", "\\ab".EscapeUnicode());
			Assert.AreEqual("\\a\\b\\u23556", "\\a\\b\u23556".EscapeUnicode());
			Assert.AreEqual("\\a\\b\\U23556", "\\a\\b\\U23556".EscapeUnicode());
		}
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.Reverse"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestReverse()
		{
			Assert.AreEqual("hsilgnE", "English".Reverse());
			Assert.AreEqual("文中hsilgnE", "English中文".Reverse());
			Assert.AreEqual("\u0061\u0308文中hsilgnE", "English中文\u0061\u0308".Reverse());
			Assert.AreEqual("\u0061\u0308文中hsilgnE\U0001D160", "\U0001D160English中文\u0061\u0308".Reverse());
		}
	}
}
