﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cyjb;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.Reflection
{
	/// <summary>
	/// <see cref="PowerBinder"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestPowerBinder
	{
		/// <summary>
		/// 测试调用字段。
		/// </summary>
		[TestMethod]
		public void TestInvokeMemberField()
		{
			var type = typeof(TestClass);
			var subType = typeof(TestSubClass);
			var targetSub = new TestSubClass();
			TestClass target = targetSub;
			// 测试父类。
			// Default
			Assert.AreEqual(target.TestField, type.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(target.TestField2, type.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(target.TestField3, type.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(target.TestField4, type.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			// Explicit
			Assert.AreEqual(target.TestField, type.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(target.TestField2, type.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(target.TestField3, type.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(target.TestField4, type.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			// 测试子类。
			// Default
			Assert.AreEqual(targetSub.TestField, subType.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField2, subType.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField3, subType.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField4, subType.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.Default,
				target, new object[0]));
			// Explicit
			Assert.AreEqual(targetSub.TestField, subType.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField2, subType.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField3, subType.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			Assert.AreEqual(targetSub.TestField4, subType.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.Explicit,
				target, new object[0]));
			// 测试设置父类字段。
			// Default
			type.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.Default, target, new object[] { "TestClass0" });
			Assert.AreEqual("TestClass0", target.TestField);
			type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default, target, new object[] { (short)20 });
			Assert.AreEqual((short)20, target.TestField2);
			AssertExt.ThrowsException(() => type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default,
				target, new object[] { 20 }), typeof(ArgumentException), typeof(InvalidCastException));
			type.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Default, target, new object[] { 200 });
			Assert.AreEqual(200L, target.TestField3);
			type.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Default, target, new object[] { "TestClass44" });
			Assert.AreEqual("TestClass44", target.TestField4);
			// Explicit
			type.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { "TestClass0" });
			Assert.AreEqual("TestClass0", target.TestField);
			type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 20 });
			Assert.AreEqual((short)20, target.TestField2);
			type.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 200 });
			Assert.AreEqual(200L, target.TestField3);
			type.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { "TestClass44" });
			Assert.AreEqual("TestClass44", target.TestField4);
			// 测试设置子类字段。
			// Default
			subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.Default, target,
				new object[] { "TestSubClass0" });
			Assert.AreEqual("TestSubClass0", targetSub.TestField);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default, target, new object[] { 21 });
			Assert.AreEqual(21, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Default, target, new object[] { 201 });
			Assert.AreEqual(201, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Default, target, new object[] { 202 });
			Assert.AreEqual(202, targetSub.TestField4);
			// Explicit
			subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.Explicit, target,
				new object[] { "TestSubClass0" });
			Assert.AreEqual("TestSubClass0", targetSub.TestField);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 21 });
			Assert.AreEqual(21, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 201 });
			Assert.AreEqual(201, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 202 });
			Assert.AreEqual(202, targetSub.TestField4);
			// 测试设置子类和父类字段。
			// Default
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default, target, new object[] { (short)22 });
			Assert.AreEqual((short)22, target.TestField2);
			AssertExt.ThrowsException(() => subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default,
				target, new object[] { 23L }),
				typeof(MissingFieldException), typeof(ArgumentException), typeof(InvalidCastException));
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Default, target, new object[] { 202L });
			Assert.AreEqual(202L, target.TestField3);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Default, target, new object[] { (short)203 });
			Assert.AreEqual(203, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Default, target,
				new object[] { "TestClass00" });
			Assert.AreEqual("TestClass00", target.TestField4);
			// Explicit
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { (short)22 });
			Assert.AreEqual((short)22, target.TestField2);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 23L });
			Assert.AreEqual(23, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { 202L });
			Assert.AreEqual(202L, target.TestField3);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.Explicit, target, new object[] { (short)203 });
			Assert.AreEqual(203, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.Explicit, target,
				new object[] { "TestClass00" });
			Assert.AreEqual("TestClass00", target.TestField4);
			// 测试强制类型转换。
			// Default
			AssertExt.ThrowsException(() =>
				subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Default, target,
				new object[] { Tristate.True }),
				typeof(MissingFieldException), typeof(ArgumentException), typeof(InvalidCastException));
			// Explicit
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.Explicit, target,
				new object[] { Tristate.True });
			Assert.AreEqual((int)Tristate.True, targetSub.TestField2);
			AssertExt.ThrowsException(() =>
				subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.Explicit, target,
				new object[] { 22 }), typeof(MissingFieldException));
		}
		/// <summary>
		/// 测试调用方法。
		/// </summary>
		[TestMethod]
		public void TestInvokeMemberMethod()
		{
			var type = typeof(TestSubClass);
			var bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
			var bindingOptFlags = bindingFlags | BindingFlags.OptionalParamBinding;
			// 测试完整的调用。
			Assert.AreEqual(TestSubClass.TestMethod(),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[0]));
			Assert.AreEqual(TestSubClass.TestMethod(10),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10 }));
			Assert.AreEqual(TestSubClass.TestMethod(10),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Explicit, null,
				new object[] { 10 }));
			Assert.AreEqual(TestSubClass.TestMethod(10, 20),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, 20 }));
			Assert.AreEqual(TestSubClass.TestMethod(10, 20),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Explicit, null,
				new object[] { 10, 20 }));
			Assert.AreEqual(TestSubClass.TestMethod(30, null, true),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 30, null, true }));
			// 测试完整的调用与类型转换。
			Type[] invalidCastExceptions = { 
				typeof(ArgumentException), typeof(InvalidCastException), typeof(MissingMethodException) };
			Assert.AreEqual(TestSubClass.TestMethod(10),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10 }));
			Assert.AreEqual(TestSubClass.TestMethod(10),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Explicit, null,
				new object[] { (short)10 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10L }), invalidCastExceptions);
			Assert.AreEqual(TestSubClass.TestMethod(10),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Explicit, null,
				new object[] { 10L }));
			Assert.AreEqual(TestSubClass.TestMethod(10, 20),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10, (byte)20 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10UL, 20L }), invalidCastExceptions);
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, 20L }), invalidCastExceptions);
			Assert.AreEqual(TestSubClass.TestMethod(10, 20),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Explicit, null,
				new object[] { 10UL, 20L }));
			// 测试命名参数。
			Assert.AreEqual(TestSubClass.TestMethod(30, "str", true),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true }));
			Assert.AreEqual(TestSubClass.TestMethod(value2: "str", value1: 30, value3: true),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { "str", 30, true }, null, null, new[] { "value2", "value1", "value3" }));
			Assert.AreEqual(TestSubClass.TestMethod(30, "str", true),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { "str", 30, true }, null, null, new[] { "value2" }));
			Assert.AreEqual(TestSubClass.TestMethod(value3: true, value2: "str", value1: 30),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { true, "str", 30 }, null, null, new[] { "value3", "value2", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod(30, "str", true),
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { true, "str", 30 }, null, null, new[] { "value3", "value2" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.Default, null,
				new object[] { 10 }, null, null, new[] { "values" }), typeof(MissingMethodException));
			// 测试默认参数和 params 参数。
			Assert.AreEqual(TestSubClass.TestMethod2(30, "str", true, 1, 2, 3),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true, 1, 2, 3 }));
			Assert.AreEqual(TestSubClass.TestMethod2(30, "str", true),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true }));
			Assert.AreEqual(TestSubClass.TestMethod2(30, "str"),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str" }));
			Assert.AreEqual(TestSubClass.TestMethod2(30),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[0]), typeof(MissingMethodException));
			// 测试命名参数、默认参数和 params 参数。
			Assert.AreEqual(TestSubClass.TestMethod2(value1: 30, value2: "str", value3: true, value4: new[] { 1, 2, 3 }),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true, 1, 2, 3 }, null, null,
				new[] { "value1", "value2", "value3", "value4" }));
			Assert.AreEqual(TestSubClass.TestMethod2(30, "str", true, 1, 2, 3),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true, 1, 2, 3 }, null, null,
				new[] { "value1", "value2", "value3" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value1: 30, value2: "str", value3: true),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", true }, null, null,
				new[] { "value1", "value2", "value3" }));
			Assert.AreEqual(TestSubClass.TestMethod2(30, "str", true, 1, 2, 3),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", 1, true, 2, 3 }, null, null,
				new[] { "value1", "value2", "value4" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value1: 30, value2: "str", value4: new[] { 1 }),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", new[] { 1 } }, null, null,
				new[] { "value1", "value2", "value4" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value4: new[] { 1 }, value3: true, value1: 30),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { new[] { 1 }, true, 30 }, null, null,
				new[] { "value4", "value3", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value4: new[] { 1 }, value1: 30),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { new[] { 1 }, 30 }, null, null,
				new[] { "value4", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value3: true, value1: 30),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { true, 30 }, null, null,
				new[] { "value3", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod2(value1: 30),
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30 }, null, null,
				new[] { "value1" }));
			// 测试泛型方法。
			Assert.AreEqual(TestSubClass.TestMethod3(10, 20),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, 20 }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, 20),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10, 20 }));
			Assert.AreEqual(TestSubClass.TestMethod3((short)10, (short)20),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10, (short)20 }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, (long)20),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10, (long)20 }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, "str"),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, "str" }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, "str"),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { (short)10, "str" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { (long)10, "str" }), typeof(MissingMethodException));
			Assert.AreEqual(TestSubClass.TestMethod3("text", "str"),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { "text", "str" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { "str", 10 }), typeof(MissingMethodException));
			Assert.AreEqual(TestSubClass.TestMethod3("text", "str", 10),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { "text", "str", 10 }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, 20, 30),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, 20, 30 }));
			Assert.AreEqual(TestSubClass.TestMethod3(10, 20, "str"),
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.Default, null,
				new object[] { 10, 20, "str" }));
			Assert.AreEqual("<System.Int32, System.String, System.Int32>(10, str, 20,30)",
				type.InvokeMember("TestMethod5", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 10, "str", 20, 30 }));
			Assert.AreEqual("<System.Int32, System.String, System.Int64>(10, str, 20,30)",
				type.InvokeMember("TestMethod5", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 10, "str", 20L, 30 }));
			Assert.AreEqual("<System.Int32, System.String, System.Int32>(10, test, 20,30)",
				type.InvokeMember("TestMethod5", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 10, new[] { 20, 30 }, "test" }, null, null,
				new[] { "value1", "value3", "value2" }));
			// 测试选择方法。
			var bindingInsFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;
			var bindingInsOptFlags = bindingInsFlags | BindingFlags.OptionalParamBinding;
			var subClass = new TestSubClass();
			Assert.AreEqual(subClass.TestMethod4(10, 20),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, 20 }));
			Assert.AreEqual(subClass.TestMethod4(true, "str"),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { true, "str" }));
			Assert.AreEqual(subClass.TestMethod4(10, "str"),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str" }));
			Assert.AreEqual(subClass.TestMethod4(10, "str"),
				type.InvokeMember("TestMethod4", bindingInsOptFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str" }));
			Assert.AreEqual(subClass.TestMethod4(10, "str", false),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str", false }));
			Assert.AreEqual(subClass.TestMethod4(10, "str", 1),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str", 1 }));
			Assert.AreEqual(subClass.TestMethod4(10, "str", 1),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str", new[] { 1 } }));
			Assert.AreEqual(subClass.TestMethod4(10, "str", 1, 2),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Explicit, subClass,
				new object[] { 10, "str", 1, 2 }));
			Assert.AreEqual(subClass.TestMethod4(true, true),
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.Default, subClass,
				new object[] { true, true }));
			Assert.AreEqual(TestSubClass.TestMethod6(value2: 30, value1: "str"),
				type.InvokeMember("TestMethod6", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str" }, null, null,
				new[] { "value2", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod6(value2: (short)30, value1: "str"),
				type.InvokeMember("TestMethod6", bindingOptFlags, PowerBinder.Default, null,
				new object[] { (short)30, "str" }, null, null,
				new[] { "value2", "value1" }));
			Assert.AreEqual(TestSubClass.TestMethod6(value2: 30, value1: "str", value3: "str2"),
				type.InvokeMember("TestMethod6", bindingOptFlags, PowerBinder.Default, null,
				new object[] { 30, "str", "str2" }, null, null,
				new[] { "value2", "value1" }));
		}
		/// <summary>
		/// 测试选择方法。
		/// </summary>
		[TestMethod]
		public void TestSelectProperty()
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			var type = typeof(TestSubClass);
			var testProperty2 = type.GetProperty("TestProperty2", typeof(short));
			var testProperty3 = type.GetProperty("TestProperty3", typeof(long));
			var testProperty4 = type.GetProperty("TestProperty4", typeof(string));
			var subTestProperty = type.GetProperty("TestProperty", typeof(string));
			var subTestProperty2 = type.GetProperty("TestProperty2", typeof(int));
			var subTestProperty3 = type.GetProperty("TestProperty3", typeof(int));
			var subTestProperty4 = type.GetProperty("TestProperty4", typeof(int));
			// TestProperty
			Assert.AreEqual(subTestProperty, type.GetProperty("TestProperty", bindingFlags, PowerBinder.Default,
				typeof(string), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty, type.GetProperty("TestProperty", bindingFlags, PowerBinder.Explicit,
				typeof(string), Type.EmptyTypes, null));
			// TestProperty2
			Assert.AreEqual(subTestProperty2, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Default,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty2, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Explicit,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty2, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Default,
				typeof(short), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty2, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Explicit,
				typeof(short), Type.EmptyTypes, null));
			Assert.AreEqual(null, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Default,
				typeof(long), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty2, type.GetProperty("TestProperty2", bindingFlags, PowerBinder.Explicit,
				typeof(long), Type.EmptyTypes, null));
			// TestProperty3
			Assert.AreEqual(subTestProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Default,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Explicit,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Default,
				typeof(long), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Explicit,
				typeof(long), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Default,
				typeof(short), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty3, type.GetProperty("TestProperty3", bindingFlags, PowerBinder.Explicit,
				typeof(short), Type.EmptyTypes, null));
			// TestProperty4
			Assert.AreEqual(subTestProperty4, type.GetProperty("TestProperty4", bindingFlags, PowerBinder.Default,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(subTestProperty4, type.GetProperty("TestProperty4", bindingFlags, PowerBinder.Explicit,
				typeof(int), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty4, type.GetProperty("TestProperty4", bindingFlags, PowerBinder.Default,
				typeof(string), Type.EmptyTypes, null));
			Assert.AreEqual(testProperty4, type.GetProperty("TestProperty4", bindingFlags, PowerBinder.Explicit,
				typeof(string), Type.EmptyTypes, null));
		}
		/// <summary>
		/// 测试辅助类。
		/// </summary>
		private class TestClass
		{
			public string TestField = "TestClass";
			public short TestField2 = 10;
			public long TestField3 = 100;
			public string TestField4 = "TestClass4";
			public string TestProperty { get; set; }
			public short TestProperty2 { get; set; }
			public long TestProperty3 { get; set; }
			public string TestProperty4 { get; set; }
			public object TestMethod4(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public object TestMethod4(bool value1, string value2)
			{
				return string.Concat("(bool ", value1, ", string ", value2, ")");
			}
		}
		private class TestSubClass : TestClass
		{
			public new string TestField = "TestSubClass";
			public new int TestField2 = 11;
			public new int TestField3 = 101;
			public new int TestField4 = 102;
			public new string TestProperty { get; set; }
			public new int TestProperty2 { get; set; }
			public new int TestProperty3 { get; set; }
			public new int TestProperty4 { get; set; }
			public static object TestMethod()
			{
				return "()";
			}
			public static object TestMethod(int value)
			{
				return string.Concat("(int ", value, ")");
			}
			public static object TestMethod(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public static object TestMethod(int value1, string value2, bool value3)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3, ")");
			}
			public static object TestMethod2(int value1, string value2 = "text", bool value3 = false, params int[] value4)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3,
					", int[] ", string.Join(",", value4), ")");
			}
			public static object TestMethod3(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public static object TestMethod3(int value1, string value2)
			{
				return string.Concat("(int ", value1, ", string ", value2, ")");
			}
			public static object TestMethod3<T>(T value1, T value2)
			{
				return string.Concat("<", typeof(T), ">(", value1, ", ", value2, ")");
			}
			public static object TestMethod3<T1, T2>(T1 value1, T1 value2, T2 value3)
			{
				return string.Concat("<", typeof(T1), ", ", typeof(T2), ">(", value1, ", ", value2, ", ", value3, ")");
			}
			public static object TestMethod3<T1>(T1 value1, T1 value2, string value3)
			{
				return string.Concat("<", typeof(T1), ">(", value1, ", ", value2, ", string ", value3, ")");
			}
			public new object TestMethod4(int value1, int value2)
			{
				return string.Concat("sub(int ", value1, ", int ", value2, ")");
			}
			public object TestMethod4(int value1, string value2)
			{
				return string.Concat("(int ", value1, ", string ", value2, ")");
			}
			public object TestMethod4(int value1, string value2, bool value3 = true)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3, ")");
			}
			public object TestMethod4(int value1, string value2, params int[] value3)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", int[] ", string.Join(",", value3), ")");
			}
			public object TestMethod4<T>(T value1, T value2)
			{
				return string.Concat("<", typeof(T), ">(", value1, ", ", value2, ")");
			}
			public static string TestMethod5<T, T2, T3>(T value1, T2 value2 = null, params T3[] value3)
				where T2 : class
			{
				return string.Concat("<", typeof(T), ", ", typeof(T2), ", ", typeof(T3), ">(",
					value1, ", ", value2, ", ", string.Join(",", value3), ")");
			}
			public static string TestMethod6(string value1, short value2)
			{
				return string.Concat("(string ", value1, ", short ", value2, ")");
			}
			public static string TestMethod6(string value1, int value2)
			{
				return string.Concat("(string ", value1, ", int ", value2, ")");
			}
			public static string TestMethod6(int value2, string value1, string value3 = "text")
			{
				return string.Concat("(int ", value2, ", string ", value1, ", string ", value3, ")");
			}
		}
	}
}
