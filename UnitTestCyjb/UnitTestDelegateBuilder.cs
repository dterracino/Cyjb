﻿using System;
using System.Reflection;
using System.Text;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
    /// <summary>
    /// <see cref="Cyjb.DelegateBuilder"/> 类的单元测试。
    /// </summary>
    [TestClass]
	public class UnitTestDelegateBuilder
	{

		#region 测试通用委托

		/// <summary>
		/// 测试构造 MethodInvoker 委托。
		/// </summary>
		[TestMethod]
		public void TestMethodInvoker()
		{
			var type = typeof(TestClass);

			// 静态方法
			// 无参数
			var invoker = type.GetMethod("TestStaticMethod", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("StaticMethod", invoker("NoUse"));
			Assert.AreEqual("StaticMethod", invoker(null));
			// 字符串参数
			invoker = type.GetMethod("TestStaticMethod", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_StaticMethod", invoker("NoUse", "Test"));
			Assert.AreEqual("Test_StaticMethod", invoker(null, "Test"));
			AssertExt.ThrowsException(() => invoker("NoUse", null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => invoker("NoUse"), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => invoker("NoUse", "Test", "more args"), typeof(TargetParameterCountException));
			// 整数参数
			invoker = type.GetMethod("TestStaticMethod", new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10_StaticMethod", invoker("NoUse", 10));
			Assert.AreEqual("10_StaticMethod", invoker(null, 10));
			// 可变参数
			invoker = type.GetMethod("TestStaticMethodVarargs", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("StaticMethod", invoker(null));
			invoker = type.GetMethod("TestStaticMethodVarargs", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_StaticMethod", invoker(null, "Test"));
			// 引用参数
			invoker = type.GetMethod("TestStaticMethodRef").CreateDelegate();
			Assert.AreEqual("A_B_StaticMethod", invoker(null, "A", "B", 0));

			// 实例方法
			var instance = new TestClass { Text = "Instance" };
			// 无参数
			invoker = type.GetMethod("TestInstanceMethod", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("Instance_InstanceMethod", invoker(instance));
			AssertExt.ThrowsException(() => invoker(null), typeof(ArgumentNullException));
			// 字符串参数
			invoker = type.GetMethod("TestInstanceMethod", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_Instance_InstanceMethod", invoker(instance, "Test"));
			AssertExt.ThrowsException(() => invoker(instance, null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => invoker(instance), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => invoker(instance, "Test", "more args"), typeof(TargetParameterCountException));
			// 整数参数
			invoker = type.GetMethod("TestInstanceMethod", new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10_Instance_InstanceMethod", invoker(instance, 10));
			// 可变参数
			invoker = type.GetMethod("TestInstanceMethodVarargs", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("Instance_InstanceMethod", invoker(instance));
			invoker = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_Instance_InstanceMethod", invoker(instance, "Test"));
			// 引用参数
			invoker = type.GetMethod("TestInstanceMethodRef").CreateDelegate();
			Assert.AreEqual("A_B_InstanceMethod", invoker(instance, "A", "B", 0));

			// ToString
			invoker = typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker(10));
			Assert.AreEqual("10", invoker("10"));
			invoker = typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker("10"));
			invoker = typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker(10));
		}
		/// <summary>
		/// 测试构造 InstanceCreator 委托。
		/// </summary>
		[TestMethod]
		public void TestInstanceCreator()
		{
			var type = typeof(TestClass);

			// 无参数
			var creator = type.GetConstructor(Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("NoParam", ((TestClass)creator()).Text);
			// 字符串参数
			creator = type.GetConstructor(new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test", ((TestClass)creator("Test")).Text);
			AssertExt.ThrowsException(() => creator(null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => creator(), typeof(TargetParameterCountException));
			// 整数参数
			creator = type.GetConstructor(new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10", ((TestClass)creator(10)).Text);

			// 默认构造函数
			Assert.AreEqual("NoParam", ((TestClass)typeof(TestClass).CreateInstanceCreator()()).Text);
			Assert.AreEqual(0, typeof(int).CreateInstanceCreator()());
			Assert.AreEqual(typeof(TestStruct), typeof(TestStruct).CreateInstanceCreator()().GetType());
		}

		#endregion // 测试通用委托

		#region 测试开放方法委托

		/// <summary>
		/// 测试构造开放方法委托。
		/// </summary>
		[TestMethod]
		public void TestOpenMethodDelegate()
		{
			var type = typeof(TestClass);

			// 静态方法
			// 无参数
			MethodBase method = type.GetMethod("TestStaticMethod", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>()());
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, method.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(false));
			// 字符串参数
			MethodBase methodStr = type.GetMethod("TestStaticMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, string>>()("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, object>>()("Test"));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<int, string>>(false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, int>>(false));
			// 整数参数
			MethodBase methodInt = type.GetMethod("TestStaticMethod", new[] { typeof(int) });
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<int, string>>()(10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<long, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<short, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, object>>()(10));
			// 可选参数
			MethodBase methodOptional = type.GetMethod("TestStaticMethodOptional");
			Assert.AreEqual("Test_10_StaticMethod", methodOptional.CreateDelegate<Func<string, int, string>>()("Test", 10));
			Assert.AreEqual("Test_0_StaticMethod", methodOptional.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("defaultKey_0_StaticMethod", methodOptional.CreateDelegate<Func<string>>()());
			// 可变参数
			method = type.GetMethod("TestStaticMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>()());
			Assert.AreEqual("Test_StaticMethod", method.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test_Test2_StaticMethod", method.CreateDelegate<Func<string, string, string>>()("Test", "Test2"));
			methodStr = type.GetMethod("TestStaticMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test2_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string>>()(
				"Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string, string>>()(
				"Test", "Test2", "Test3"));
			// 泛型方法
			MethodBase methodGeneric = type.GetMethod("TestStaticMethodGeneric");
			Assert.AreEqual("<System.String>Test_StaticMethod", methodGeneric.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("<System.Int32>10_StaticMethod", methodGeneric.CreateDelegate<Func<int, string>>()(10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<string, int>>(false));
			// 引用参数
			MethodBase methodRef = type.GetMethod("TestStaticMethodRef");
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<Func<string, string, int, string>>()("A", "B", 0));
			var value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<TestDelegate>()("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);

			// 实例方法
			var instance = new TestClass { Text = "Instance" };
			// 无参数
			method = type.GetMethod("TestInstanceMethod", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>()(instance));
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<object, object>>()(instance));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, string, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, int>>(false));
			// 字符串参数
			methodStr = type.GetMethod("TestInstanceMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, int, string>>(false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, string, int>>(false));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, object>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<object, object, object>>()(
				instance, "Test"));
			// 整数参数
			methodInt = type.GetMethod("TestInstanceMethod", new[] { typeof(int) });
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, int, string>>()(instance, 10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<TestClass, string, string>>(false));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, long, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, short, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, object>>()(
				instance, 10));
			// 可选参数
			methodOptional = type.GetMethod("TestInstanceMethodOptional");
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, int, string>>()(instance, "Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, string>>()(instance, "Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string>>()(instance));
			// 可变参数
			method = type.GetMethod("TestInstanceMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>()(instance));
			Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Test2_Instance_InstanceMethod",
				method.CreateDelegate<Func<TestClass, string, string, string>>()(instance, "Test", "Test2"));
			methodStr = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test2_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string>>()(instance, "Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string, string>>()(instance, "Test", "Test2", "Test3"));
			// 泛型方法
			methodGeneric = type.GetMethod("TestInstanceMethodGeneric");
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, string, string>>()(instance, "Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, int, string>>()(instance, 10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<TestClass, string, int>>(false));
			// 引用参数
			methodRef = type.GetMethod("TestInstanceMethodRef");
			Assert.AreEqual("A_B_InstanceMethod",
				methodRef.CreateDelegate<Func<TestClass, string, string, int, string>>()(instance, "A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", methodRef.CreateDelegate<TestInstanceDelegate>()(
				instance, "A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);

			// ToString
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>()(
				10));
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>()(
				"10"));
			Assert.AreEqual("10", typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string, string>>()(
				"10"));
			Assert.AreEqual("10", typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>()(
				"10"));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<int, string>>()(
				10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<long, string>>()(
				10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<short, string>>()(
				10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>()(
				10));
		}
		/// <summary>
		/// 测试构造开放构造函数委托。
		/// </summary>
		[TestMethod]
		public void TestOpenConstructorDelegate()
		{
			var type = typeof(TestClass);

			// 无参数
			MethodBase method = type.GetConstructor(Type.EmptyTypes);
			Assert.AreEqual("NoParam", method.CreateDelegate<Func<TestClass>>()().Text);
			Assert.AreEqual("NoParam", ((TestClass)method.CreateDelegate<Func<object>>()()).Text);
			Assert.AreEqual(null, method.CreateDelegate<Func<string, object>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(false));
			// 字符串参数
			MethodBase methodStr = type.GetConstructor(new[] { typeof(string) });
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<string, TestClass>>()("Test").Text);
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<object, TestClass>>()("Test").Text);
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, string>>(false));
			// 整数
			MethodBase methodInt = type.GetConstructor(new[] { typeof(int) });
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<int, TestClass>>()(10).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<ulong, TestClass>>()(10UL).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<short, TestClass>>()(10).Text);
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, TestClass>>(false));
		}

		#endregion // 测试开放方法委托

		#region 测试封闭方法委托

		/// <summary>
		/// 测试构造封闭方法委托。
		/// </summary>
		[TestMethod]
		public void TestClosedMethodDelegate()
		{
			var type = typeof(TestClass);

			// 静态方法
			// 无参数
			MethodBase method = type.GetMethod("TestStaticMethod", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<object>>(null)());
			Assert.AreEqual(null, method.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(null, false));
			// 字符串参数
			MethodBase methodStr = type.GetMethod("TestStaticMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>(null)("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, string>>(null)("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, object>>(null)("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string>>("Test")());
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object>>("Test")());
			Assert.AreEqual("_StaticMethod", methodStr.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("_StaticMethod", methodStr.CreateDelegate<Func<object>>(null)());
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<int, string>>(null, false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, int>>(null, false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<int>>(null, false));
			// 整数参数
			MethodBase methodInt = type.GetMethod("TestStaticMethod", new[] { typeof(int) });
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<int, string>>(null)(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<string>>(10)());
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<long, string>>(null)(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<short, string>>(null)(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, string>>(null)(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, object>>(null)(10));
			// 可选参数
			MethodBase methodOptional = type.GetMethod("TestStaticMethodOptional");
			Assert.AreEqual("Test_10_StaticMethod", methodOptional.CreateDelegate<Func<string, int, string>>(null)("Test", 10));
			Assert.AreEqual("Test_0_StaticMethod", methodOptional.CreateDelegate<Func<string, string>>(null)("Test"));
			Assert.AreEqual("defaultKey_0_StaticMethod", methodOptional.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("Test_10_StaticMethod", methodOptional.CreateDelegate<Func<int, string>>("Test")(10));
			Assert.AreEqual("Test_0_StaticMethod", methodOptional.CreateDelegate<Func<string>>("Test")());
			// 可变参数
			method = type.GetMethod("TestStaticMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("Test_StaticMethod", method.CreateDelegate<Func<string, string>>(null)("Test"));
			Assert.AreEqual("Test_Test2_StaticMethod", method.CreateDelegate<Func<string, string, string>>
				(null)("Test", "Test2"));
			methodStr = type.GetMethod("TestStaticMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>(null)("Test"));
			Assert.AreEqual("Test2_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string>>
				(null)("Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string, string>>
				(null)("Test", "Test2", "Test3"));
			Assert.AreEqual("Test_StaticMethod", method.CreateDelegate<Func<string>>("Test")());
			Assert.AreEqual("Test_Test2_StaticMethod", method.CreateDelegate<Func<string, string>>("Test")("Test2"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string>>("Test")());
			Assert.AreEqual("Test2_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>("Test")("Test2"));
			Assert.AreEqual("Test2_Test3_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string>>("Test")
				("Test2", "Test3"));
			// 泛型方法
			MethodBase methodGeneric = type.GetMethod("TestStaticMethodGeneric");
			Assert.AreEqual("<System.String>Test_StaticMethod", methodGeneric.CreateDelegate<Func<string, string>>(null)("Test"));
			Assert.AreEqual("<System.Int32>10_StaticMethod", methodGeneric.CreateDelegate<Func<int, string>>(null)(10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<string, int>>(null, false));
			Assert.AreEqual("<System.String>Test_StaticMethod", methodGeneric.CreateDelegate<Func<string>>("Test")());
			Assert.AreEqual("<System.Int32>10_StaticMethod", methodGeneric.CreateDelegate<Func<string>>(10)());
			// 引用参数
			MethodBase methodRef = type.GetMethod("TestStaticMethodRef");
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<Func<string, string, int, string>>(null)("A", "B", 0));
			var value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<TestDelegate>(null)("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<Func<string, int, string>>("A")("B", 0));
			value = "B";
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<TestDelegateWithoutKey>("A")(ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);

			// 实例方法
			var instance = new TestClass { Text = "Instance" };
			// 无参数
			method = type.GetMethod("TestInstanceMethod", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>(null)(instance));
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<object, object>>(null)(instance));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, string, string>>(null, false));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, int>>(null, false));
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<string>>(instance)());
			// 字符串参数
			methodStr = type.GetMethod("TestInstanceMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, string, string>>(null)(
				instance, "Test"));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, int, string>>(null, false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, string, int>>(null, false));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, string>>(null)(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, object>>(null)(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<object, object, object>>(null)(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<string, string>>(instance)(
				"Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<object, string>>(instance)(
				"Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<object, object>>(instance)(
				"Test"));
			// 整数参数
			methodInt = type.GetMethod("TestInstanceMethod", new[] { typeof(int) });
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, int, string>>(null)(
				instance, 10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<TestClass, string, string>>(null, false));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, long, string>>(null)(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, short, string>>(null)(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, string>>(null)(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, object>>(null)(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<int, string>>(instance)(10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, string>>(instance, false));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<long, string>>(instance)(10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<short, string>>(instance)(10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<object, string>>(instance)(10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<object, object>>(instance)(10));
			// 可选参数
			methodOptional = type.GetMethod("TestInstanceMethodOptional");
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, int, string>>(null)(instance, "Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, string>>(null)(instance, "Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string>>(null)(instance));
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<string, int, string>>(instance)("Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<string, string>>(instance)("Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<string>>(instance)());
			// 可变参数
			method = type.GetMethod("TestInstanceMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>(null)(instance));
			Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>(null)(
				instance, "Test"));
			Assert.AreEqual("Test_Test2_Instance_InstanceMethod",
				method.CreateDelegate<Func<TestClass, string, string, string>>(null)(instance, "Test", "Test2"));
			methodStr = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string>>(null)(instance, "Test"));
			Assert.AreEqual("Test2_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string>>(null)(instance, "Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string, string>>(null)(
				instance, "Test", "Test2", "Test3"));
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<string>>(instance)());
			Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<string, string>>(instance)(
				"Test"));
			Assert.AreEqual("Test_Test2_Instance_InstanceMethod",
				method.CreateDelegate<Func<string, string, string>>(instance)("Test", "Test2"));
			methodStr = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string>>(null)(instance, "Test"));
			Assert.AreEqual("Test2_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string>>(null)(instance, "Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string, string>>(null)(
				instance, "Test", "Test2", "Test3"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<string, string>>(instance)(
				"Test"));
			Assert.AreEqual("Test2_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<string, string, string>>(instance)("Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<string, string, string, string>>(instance)("Test", "Test2", "Test3"));
			// 泛型方法
			methodGeneric = type.GetMethod("TestInstanceMethodGeneric");
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, string, string>>(null)(instance, "Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, int, string>>(null)(instance, 10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<TestClass, string, int>>(null, false));
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<string, string>>(instance)("Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<int, string>>(instance)(10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<string, int>>(instance, false));
			// 引用参数
			methodRef = type.GetMethod("TestInstanceMethodRef");
			Assert.AreEqual("A_B_InstanceMethod",
				methodRef.CreateDelegate<Func<TestClass, string, string, int, string>>(null)(instance, "A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", methodRef.CreateDelegate<TestInstanceDelegate>(null)(
				instance, "A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);
			Assert.AreEqual("A_B_InstanceMethod",
				methodRef.CreateDelegate<Func<string, string, int, string>>(instance)("A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", methodRef.CreateDelegate<TestDelegate>(instance)(
				"A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);

			// ToString
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>(
				null)(10));
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>(
				null)("10"));
			Assert.AreEqual("10", typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string, string>>(
				null)("10"));
			Assert.AreEqual("10", typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>(
				null)("10"));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<int, string>>(
				null)(10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<long, string>>(
				null)(10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<short, string>>(
				null)(10));
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<object, string>>(
				null)(10));
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>(10)());
			Assert.AreEqual("10", typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>("10")());
			Assert.AreEqual("UnitTestCyjb.UnitTestDelegateBuilder+TestStruct",
				typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>(new TestStruct())());
			Assert.AreEqual("10", typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>("10")());
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>(10)());
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>(10L)());
			Assert.AreEqual("10", typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>((short)10)());
			Assert.AreEqual("UnitTestCyjb.UnitTestDelegateBuilder+TestStruct",
				typeof(TestStruct).GetMethod("ToString", Type.EmptyTypes).CreateDelegate<Func<string>>(new TestStruct())());
		}
		/// <summary>
		/// 测试构造封闭构造函数委托。
		/// </summary>
		[TestMethod]
		public void TestClosedConstructorDelegate()
		{
			var type = typeof(TestClass);

			// 无参数
			MethodBase method = type.GetConstructor(Type.EmptyTypes);
			Assert.AreEqual("NoParam", method.CreateDelegate<Func<TestClass>>(null)().Text);
			Assert.AreEqual("NoParam", ((TestClass)method.CreateDelegate<Func<object>>(null)()).Text);
			Assert.AreEqual(null, method.CreateDelegate<Func<string, object>>(null, false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(null, false));
			// 字符串参数
			MethodBase methodStr = type.GetConstructor(new[] { typeof(string) });
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<string, TestClass>>(null)("Test").Text);
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<object, TestClass>>(null)("Test").Text);
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<TestClass>>("Test")().Text);
			// 整数
			MethodBase methodInt = type.GetConstructor(new[] { typeof(int) });
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<int, TestClass>>(null)(10).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<ulong, TestClass>>(null)(10UL).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<short, TestClass>>(null)(10).Text);
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, TestClass>>(null, false));
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<TestClass>>(10)().Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<TestClass>>(10UL)().Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<TestClass>>((short)10)().Text);
		}

		#endregion // 测试封闭方法委托

		#region 测试属性委托

		/// <summary>
		/// 测试构造开放属性委托。
		/// </summary>
		[TestMethod]
		public void TestOpenPropertyDelegate()
		{
			var type = typeof(TestClass);

			// 静态属性
			var property = type.GetProperty("TestStaticProperty");
			property.CreateDelegate<Action<string>>()("Test1");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<string>>()());
			property.CreateDelegate<Action<object>>()("Test2");
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, property.CreateDelegate<Func<int>>(false));

			// 实例属性
			property = type.GetProperty("TestInstanceProperty");
			var instance = new TestClass();
			property.CreateDelegate<Action<TestClass, string>>()(instance, "Test1");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<TestClass, string>>()(instance));
			property.CreateDelegate<Action<TestClass, object>>()(instance, "Test2");
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object, object>>()(instance));
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, property.CreateDelegate<Func<TestClass, int>>(false));
			// 索引属性
			property = type.GetProperty("Item", new[] { typeof(int) });
			property.CreateDelegate<Action<TestClass, int, string>>()(instance, 0, "Test1");
			property.CreateDelegate<Action<object, object, object>>(null)(instance, 1, "Test2");
			property.CreateDelegate<Action<object, long, object>>(null)(instance, 2, "Test3");
			property.CreateDelegate<Action<object, short, object>>(null)(instance, 3, "Test4");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<TestClass, int, string>>()(instance, 0));
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object, object, object>>()(instance, 1));
			Assert.AreEqual("Test3", property.CreateDelegate<Func<object, long, object>>()(instance, 2));
			Assert.AreEqual("Test4", property.CreateDelegate<Func<object, short, object>>()(instance, 3));

			property = type.GetProperty("Item", new[] { typeof(int), typeof(int) });
			property.CreateDelegate<Action<TestClass, short, ulong, string>>()(instance, 2, 2, "Test5");
			property.CreateDelegate<Action<object, object, short, object>>()(instance, 1, 5, "Test6");
			Assert.AreEqual("Test5", property.CreateDelegate<Func<TestClass, short, ulong, string>>()(instance, -1, 5));
			Assert.AreEqual("Test6", property.CreateDelegate<Func<object, object, short, object>>()(instance, 3, 3));
		}
		/// <summary>
		/// 测试构造封闭属性委托。
		/// </summary>
		[TestMethod]
		public void TestClosedPropertyDelegate()
		{
			var type = typeof(TestClass);

			// 静态属性
			var property = type.GetProperty("TestStaticProperty");
			property.CreateDelegate<Action<string>>(null)("Test1");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<string>>(null)());
			property.CreateDelegate<Action<object>>(null)("Test2");
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object>>(null)());
			property.CreateDelegate<Action>("Test3")();
			Assert.AreEqual("Test3", property.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual(null, property.CreateDelegate<Func<int>>(null, false));

			// 实例属性
			property = type.GetProperty("TestInstanceProperty");
			var instance = new TestClass();
			property.CreateDelegate<Action<string>>(instance)("Test1");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<string>>(instance)());
			property.CreateDelegate<Action<object>>(instance)("Test2");
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object>>(instance)());
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual(null, property.CreateDelegate<Func<TestClass, int>>(null, false));
			// 索引属性
			property = type.GetProperty("Item", new[] { typeof(int) });
			property.CreateDelegate<Action<int, string>>(instance)(0, "Test1");
			property.CreateDelegate<Action<object, object>>(instance)(1, "Test2");
			property.CreateDelegate<Action<long, object>>(instance)(2, "Test3");
			property.CreateDelegate<Action<short, object>>(instance)(3, "Test4");
			Assert.AreEqual("Test1", property.CreateDelegate<Func<int, string>>(instance)(0));
			Assert.AreEqual("Test2", property.CreateDelegate<Func<object, object>>(instance)(1));
			Assert.AreEqual("Test3", property.CreateDelegate<Func<long, object>>(instance)(2));
			Assert.AreEqual("Test4", property.CreateDelegate<Func<short, object>>(instance)(3));

			property = type.GetProperty("Item", new[] { typeof(int), typeof(int) });
			property.CreateDelegate<Action<short, ulong, string>>(instance)(2, 2, "Test5");
			property.CreateDelegate<Action<object, short, object>>(instance)(1, 5, "Test6");
			Assert.AreEqual("Test5", property.CreateDelegate<Func<short, ulong, string>>(instance)(-1, 5));
			Assert.AreEqual("Test6", property.CreateDelegate<Func<object, short, object>>(instance)(3, 3));
		}

		#endregion // 测试属性委托

		#region 测试字段委托

		/// <summary>
		/// 测试构造开放字段委托。
		/// </summary>
		[TestMethod]
		public void TestOpenFieldDelegate()
		{
			var type = typeof(TestClass);

			// 静态字段
			var field = type.GetField("TestStaticField");
			field.CreateDelegate<Action<string>>()("Test1");
			Assert.AreEqual("Test1", field.CreateDelegate<Func<string>>()());
			field.CreateDelegate<Action<object>>()("Test2");
			Assert.AreEqual("Test2", field.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, field.CreateDelegate<Func<int>>(false));

			// 实例字段
			field = type.GetField("TestInstanceField");
			var instance = new TestClass();
			field.CreateDelegate<Action<TestClass, string>>()(instance, "Test1");
			Assert.AreEqual("Test1", field.CreateDelegate<Func<TestClass, string>>()(instance));
			field.CreateDelegate<Action<TestClass, object>>()(instance, "Test2");
			Assert.AreEqual("Test2", field.CreateDelegate<Func<object, object>>()(instance));
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, field.CreateDelegate<Func<TestClass, int>>(false));
		}
		/// <summary>
		/// 测试构造封闭字段委托。
		/// </summary>
		[TestMethod]
		public void TestClosedFieldDelegate()
		{
			var type = typeof(TestClass);

			// 静态字段
			var field = type.GetField("TestStaticField");
			field.CreateDelegate<Action<string>>(null)("Test1");
			Assert.AreEqual("Test1", field.CreateDelegate<Func<string>>(null)());
			field.CreateDelegate<Action<object>>(null)("Test2");
			Assert.AreEqual("Test2", field.CreateDelegate<Func<object>>(null)());
			field.CreateDelegate<Action>("Test3")();
			Assert.AreEqual("Test3", field.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual(null, field.CreateDelegate<Func<int>>(null, false));

			// 实例字段
			field = type.GetField("TestInstanceField");
			var instance = new TestClass();
			field.CreateDelegate<Action<string>>(instance)("Test1");
			Assert.AreEqual("Test1", field.CreateDelegate<Func<string>>(instance)());
			field.CreateDelegate<Action<object>>(instance)("Test2");
			Assert.AreEqual("Test2", field.CreateDelegate<Func<object>>(instance)());
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(null, false));
			Assert.AreEqual(null, field.CreateDelegate<Func<TestClass, int>>(null, false));
		}

		#endregion // 测试字段委托

		#region 测试委托包装

		/// <summary>
		/// 测试构造委托包装。
		/// </summary>
		[TestMethod]
		public void TestWrapDelegate()
		{
			Action action = () => { };
			Assert.AreEqual(action, action.Wrap<Action>());
			Func<string> func = () => "Test";
			Assert.AreEqual("Test", func.Wrap<TestFunc>()());
			Func<string, int, string> func2 = (k, v) => k + "_" + v + "_Func";
			Assert.AreEqual("Test_0_Func", func2.Wrap<TestFunc2>()("Test", 0));
			var value = "Test";
			int value2;
			Assert.AreEqual("Test_0_Func", func2.Wrap<TestFunc3>()(ref value, out value2));
			Assert.AreEqual("Test", value);
			Assert.AreEqual(0, value2);
			TestFunc3 func4 = (ref string k, out int v) =>
			{
				var oldK = k;
				k = "TestNewValue";
				v = 101;
				return oldK + "_Func";
			};
			value = "Test";
			Assert.AreEqual("Test_Func", func4.Wrap<TestFunc4>()(ref value, out value2));
			Assert.AreEqual("TestNewValue", value);
			Assert.AreEqual(101, value2);
			Assert.AreEqual("Test_Func", func4.Wrap<Func<string, int, string>>()("Test", 0));
		}
		private delegate string TestFunc();
		private delegate string TestFunc2(string k, int v);
		private delegate string TestFunc3(ref string k, out int v);
		private delegate string TestFunc4(ref string k, out int v);

		#endregion // 测试委托包装

		#region 测试构造函数委托

		/// <summary>
		/// 测试默认构造函数委托。
		/// </summary>
		[TestMethod]
		public void TestDefaultConstructorDelegate()
		{
			Assert.AreEqual("NoParam", typeof(TestClass).CreateDelegate<Func<TestClass>>()().Text);
			Assert.AreEqual("NoParam", typeof(TestClass).CreateDelegate<Func<TestClass>>(".ctor")().Text);
			Assert.AreEqual(new TestStruct(), typeof(TestStruct).CreateDelegate<Func<TestStruct>>()());
			Assert.AreEqual(new TestStruct(), typeof(TestStruct).CreateDelegate<Func<TestStruct>>(".ctor")());
			Assert.AreEqual(0, typeof(int).CreateDelegate<Func<int>>()());
			Assert.AreEqual(0, typeof(int).CreateDelegate<Func<int>>(".ctor")());
		}
		/// <summary>
		/// 测试构造函数委托。
		/// </summary>
		[TestMethod]
		public void TestConstructorDelegate()
		{
			Assert.AreEqual("Test", typeof(TestClass).CreateDelegate<Func<string, TestClass>>(".ctor")("Test").Text);
			Assert.AreEqual("10", typeof(TestClass).CreateDelegate<Func<int, TestClass>>(".ctor")(10).Text);
			Assert.AreEqual("10", typeof(TestClass).CreateDelegate<Func<long, TestClass>>(".ctor")(10).Text);
			Assert.AreEqual("10", typeof(TestClass).CreateDelegate<Func<short, TestClass>>(".ctor")(10).Text);
			Assert.AreEqual("Test", typeof(TestStruct).CreateDelegate<Func<string, TestStruct>>(".ctor")("Test").Text);
			Assert.AreEqual("10", typeof(TestStruct).CreateDelegate<Func<int, TestStruct>>(".ctor")(10).Text);
			Assert.AreEqual("10", typeof(TestStruct).CreateDelegate<Func<long, TestStruct>>(".ctor")(10).Text);
			Assert.AreEqual("10", typeof(TestStruct).CreateDelegate<Func<short, TestStruct>>(".ctor")(10).Text);
			// TODO: 封闭方法也要测试
		}

		#endregion // 测试构造函数委托

		#region 测试通过 Type 构造委托

		/// <summary>
		/// 测试通过 Type 构造开放委托。
		/// </summary>
		[TestMethod]
		public void TestOpenTypeDelegate()
		{
			var type = typeof(TestClass);

			// 静态方法
			Assert.AreEqual("StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod")());
			Assert.AreEqual("Test_StaticMethod", type.CreateDelegate<Func<string, string>>("TestStaticMethod")("Test"));
			Assert.AreEqual("10_StaticMethod", type.CreateDelegate<Func<int, string>>("TestStaticMethod")(10));
			Assert.AreEqual("10_StaticMethod", type.CreateDelegate<Func<long, string>>("TestStaticMethod")(10));
			AssertExt.ThrowsException(() => type.CreateDelegate<Func<object, string>>("TestStaticMethod"),
				typeof(ArgumentException));
			// 可选参数
			Assert.AreEqual("Test_10_StaticMethod",
				type.CreateDelegate<Func<string, int, string>>("TestStaticMethodOptional")("Test", 10));
			Assert.AreEqual("Test_0_StaticMethod",
				type.CreateDelegate<Func<string, string>>("TestStaticMethodOptional")("Test"));
			Assert.AreEqual("defaultKey_0_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethodOptional")());
			// 泛型方法
			Assert.AreEqual("<System.String>Test_StaticMethod",
				type.CreateDelegate<Func<string, string>>("TestStaticMethodGeneric")("Test"));
			Assert.AreEqual("<System.Int32>10_StaticMethod",
				type.CreateDelegate<Func<int, string>>("TestStaticMethodGeneric")(10));
			// 引用参数
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestStaticMethodRef")("A", "B", 0));
			var value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<TestDelegate>("TestStaticMethodRef")("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);

			// 实例方法
			var instance = new TestClass { Text = "Instance" };
			Assert.AreEqual("Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string>>("TestInstanceMethod")(instance));
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethod")(instance, "Test"));
			Assert.AreEqual("10_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethod")(instance, 10));
			// 可选参数
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, int, string>>("TestInstanceMethodOptional")(instance, "Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethodOptional")(instance, "Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string>>("TestInstanceMethodOptional")(instance));
			// 泛型方法
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethodGeneric")(instance, "Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethodGeneric")(instance, 10));
			// 引用参数
			Assert.AreEqual("A_B_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string, int, string>>("TestInstanceMethodRef")
				(instance, "A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", type.CreateDelegate<TestInstanceDelegate>("TestInstanceMethodRef")
				(instance, "A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);

			// ToString
			Assert.AreEqual("10", typeof(object).CreateDelegate<Func<object, string>>("ToString")(10));
			Assert.AreEqual("10", typeof(object).CreateDelegate<Func<object, string>>("ToString")("10"));
			Assert.AreEqual("10", typeof(string).CreateDelegate<Func<string, string>>("ToString")("10"));
			Assert.AreEqual("10", typeof(string).CreateDelegate<Func<object, string>>("ToString")("10"));
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<int, string>>("ToString")(10));
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<long, string>>("ToString")(10));
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<short, string>>("ToString")(10));
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<object, string>>("ToString")(10));

			// 静态属性
			type.CreateDelegate<Action<string>>("TestStaticProperty")("Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestStaticProperty")());

			// 实例属性
			type.CreateDelegate<Action<TestClass, string>>("TestInstanceProperty")(instance, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<TestClass, string>>("TestInstanceProperty")(instance));

			// 索引属性
			type.CreateDelegate<Action<TestClass, int, string>>("Item")(instance, 0, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<TestClass, int, string>>("Item")(instance, 0));
			type.CreateDelegate<Action<TestClass, short, ulong, string>>("Item")(instance, 0, 1, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<TestClass, short, ulong, string>>("Item")(instance, -1, 2));

			// 静态字段
			type.CreateDelegate<Action<string>>("TestStaticField")("Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestStaticField")());

			// 实例字段
			type.CreateDelegate<Action<TestClass, string>>("TestInstanceField")(instance, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<TestClass, string>>("TestInstanceField")(instance));
		}
		/// <summary>
		/// 测试通过 Type 构造封闭委托。
		/// </summary>
		[TestMethod]
		public void TestClosedTypeDelegate()
		{
			var type = typeof(TestClass);

			// 静态方法
			Assert.AreEqual("StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod", null)());
			Assert.AreEqual("Test_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod", "Test")());
			Assert.AreEqual("10_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod", 10)());
			Assert.AreEqual("10_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod", 10L)());
			// 可选参数
			Assert.AreEqual("Test_10_StaticMethod",
				type.CreateDelegate<Func<int, string>>("TestStaticMethodOptional", "Test")(10));
			Assert.AreEqual("Test_0_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethodOptional", "Test")());
			Assert.AreEqual("defaultKey_0_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethodOptional", null)());
			// 泛型方法
			Assert.AreEqual("<System.String>Test_StaticMethod",
				type.CreateDelegate<Func<string>>("TestStaticMethodGeneric", "Test")());
			Assert.AreEqual("<System.Int32>10_StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethodGeneric", 10)());
			// 引用参数
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, int, string>>("TestStaticMethodRef", "A")("B", 0));
			var value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<TestDelegateWithoutKey>("TestStaticMethodRef", "A")(ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);

			// 实例方法
			var instance = new TestClass { Text = "Instance" };
			Assert.AreEqual("Instance_InstanceMethod", type.CreateDelegate<Func<string>>("TestInstanceMethod", instance)());
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethod", instance)("Test"));
			Assert.AreEqual("10_Instance_InstanceMethod",
				type.CreateDelegate<Func<int, string>>("TestInstanceMethod", instance)(10));
			// 可选参数
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, int, string>>("TestInstanceMethodOptional", instance)("Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethodOptional", instance)("Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				type.CreateDelegate<Func<string>>("TestInstanceMethodOptional", instance)());
			// 泛型方法
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethodGeneric", instance)("Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				type.CreateDelegate<Func<int, string>>("TestInstanceMethodGeneric", instance)(10));
			// 引用参数
			Assert.AreEqual("A_B_InstanceMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestInstanceMethodRef", instance)("A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", type.CreateDelegate<TestDelegate>("TestInstanceMethodRef", instance)
				("A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);

			// ToString
			Assert.AreEqual("10", typeof(object).CreateDelegate<Func<string>>("ToString", 10)());
			Assert.AreEqual("10", typeof(object).CreateDelegate<Func<string>>("ToString", "10")());
			Assert.AreEqual("10", typeof(string).CreateDelegate<Func<string>>("ToString", "10")());
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<string>>("ToString", 10)());
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<string>>("ToString", 10L)());
			Assert.AreEqual("10", typeof(int).CreateDelegate<Func<string>>("ToString", (short)10)());

			// 静态属性
			type.CreateDelegate<Action>("TestStaticProperty", "Test")();
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestStaticProperty", null)());

			// 实例属性
			type.CreateDelegate<Action<string>>("TestInstanceProperty", instance)("Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestInstanceProperty", instance)());

			// 索引属性
			type.CreateDelegate<Action<int, string>>("Item", instance)(0, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<int, string>>("Item", instance)(0));
			type.CreateDelegate<Action<short, ulong, string>>("Item", instance)(0, 1, "Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<short, ulong, string>>("Item", instance)(-1, 2));

			// 静态字段
			type.CreateDelegate<Action>("TestStaticField", "Test")();
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestStaticField", null)());

			// 实例字段
			type.CreateDelegate<Action<string>>("TestInstanceField", instance)("Test");
			Assert.AreEqual("Test", type.CreateDelegate<Func<string>>("TestInstanceField", instance)());
		}
		/// <summary>
		/// 测试通过对象构造封闭委托。
		/// </summary>
		[TestMethod]
		public void TestClosedObjectDelegate()
		{
			var type = typeof(TestClass);
			var instance = new TestClass { Text = "Instance" };

			// 实例方法
			Assert.AreEqual("Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string>>(instance, "TestInstanceMethod")());
			Assert.AreEqual("Test_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string, string>>(instance, "TestInstanceMethod")("Test"));
			Assert.AreEqual("10_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<int, string>>(instance, "TestInstanceMethod")(10));
			// 可选参数
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string, int, string>>(instance, "TestInstanceMethodOptional")("Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string, string>>(instance, "TestInstanceMethodOptional")("Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string>>(instance, "TestInstanceMethodOptional")());
			// 泛型方法
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string, string>>(instance, "TestInstanceMethodGeneric")("Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<int, string>>(instance, "TestInstanceMethodGeneric")(10));
			// 引用参数
			Assert.AreEqual("A_B_InstanceMethod",
				DelegateBuilder.CreateDelegate<Func<string, string, int, string>>(
				instance, "TestInstanceMethodRef")("A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			var value = "X";
			int value2;
			Assert.AreEqual("A_X_InstanceMethod", DelegateBuilder.CreateDelegate<TestDelegate>(instance, "TestInstanceMethodRef")
				("A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);

			// ToString
			Assert.AreEqual("10", DelegateBuilder.CreateDelegate<Func<string>>("10", "ToString")());
			Assert.AreEqual("10", DelegateBuilder.CreateDelegate<Func<string>>(10, "ToString")());
			Assert.AreEqual("10", DelegateBuilder.CreateDelegate<Func<string>>(10L, "ToString")());
			Assert.AreEqual("10", DelegateBuilder.CreateDelegate<Func<string>>(10, "ToString")());

			// 实例属性
			DelegateBuilder.CreateDelegate<Action<string>>(instance, "TestInstanceProperty")("Test");
			Assert.AreEqual("Test", DelegateBuilder.CreateDelegate<Func<string>>(instance, "TestInstanceProperty")());

			// 索引属性
			DelegateBuilder.CreateDelegate<Action<int, string>>(instance, "Item")(0, "Test");
			Assert.AreEqual("Test", DelegateBuilder.CreateDelegate<Func<int, string>>(instance, "Item")(0));
			DelegateBuilder.CreateDelegate<Action<short, ulong, string>>(instance, "Item")(0, 1, "Test");
			Assert.AreEqual("Test", DelegateBuilder.CreateDelegate<Func<short, ulong, string>>(instance, "Item")(-1, 2));

			// 实例字段
			DelegateBuilder.CreateDelegate<Action<string>>(instance, "TestInstanceField")("Test");
			Assert.AreEqual("Test", DelegateBuilder.CreateDelegate<Func<string>>(instance, "TestInstanceField")());
		}

		#endregion // 测试通过 Type 构造委托

		private delegate string TestDelegate(string key, ref string value, out int value2);
		private delegate string TestDelegateWithoutKey(ref string value, out int value2);
		private delegate string TestInstanceDelegate(TestClass instance, string key, ref string value, out int value2);
		private struct TestStruct
		{
			public string Text;
			public TestStruct(string text)
			{
				Text = text;
			}
			public TestStruct(int value)
			{
				Text = value.ToString();
			}
		}
		private class TestClass
		{

			#region 静态方法

			public static string TestStaticMethod()
			{
				return "StaticMethod";
			}
			public static string TestStaticMethod(string key)
			{
				return key + "_StaticMethod";
			}
			public static string TestStaticMethod(int key)
			{
				return key + "_StaticMethod";
			}
			public static string TestStaticMethodOptional(string key = "defaultKey", int value = 0)
			{
				return key + "_" + value + "_StaticMethod";
			}
			public static string TestStaticMethodVarargs(__arglist)
			{
				var args = new ArgIterator(__arglist);
				var text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append("StaticMethod");
				return text.ToString();
			}
			public static string TestStaticMethodVarargs(string key, __arglist)
			{
				var args = new ArgIterator(__arglist);
				var text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(key);
				text.Append("_StaticMethod");
				return text.ToString();
			}
			public static string TestStaticMethodGeneric<T>(T key)
			{
				return "<" + typeof(T) + ">" + key + "_StaticMethod";
			}
			public static string TestStaticMethodRef(string key, ref string value, out int value2)
			{
				var oldValue = value;
				value = "StaticMethodRef";
				value2 = 101;
				return key + "_" + oldValue + "_StaticMethod";
			}

			#endregion // 静态方法

			#region 实例方法

			public string Text;
			public string TestInstanceMethod()
			{
				return Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(string key)
			{
				return key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(int key)
			{
				return key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodOptional(string key = "defaultKey", int value = 0)
			{
				return key + "_" + value + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodVarargs(__arglist)
			{
				var args = new ArgIterator(__arglist);
				var text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(Text);
				text.Append("_InstanceMethod");
				return text.ToString();
			}
			public string TestInstanceMethodVarargs(string key, __arglist)
			{
				var args = new ArgIterator(__arglist);
				var text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(key);
				text.Append('_');
				text.Append(Text);
				text.Append("_InstanceMethod");
				return text.ToString();
			}
			public string TestInstanceMethodGeneric<T>(T key)
			{
				return "<" + typeof(T) + ">" + key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodRef(string key, ref string value, out int value2)
			{
				Text = value;
				value = "InstanceMethodRef";
				value2 = 101;
				return key + "_" + Text + "_InstanceMethod";
			}

			#endregion // 实例方法

			#region 构造函数

			public TestClass()
			{
				Text = "NoParam";
			}
			public TestClass(string key)
			{
				Text = key;
			}
			public TestClass(int key)
			{
				Text = key.ToString();
			}

			#endregion // 构造函数

			#region 静态属性

			public static string TestStaticProperty { get; set; }

			#endregion // 静态属性

			#region 实例属性

			public string TestInstanceProperty { get; set; }
			private string[] items = new string[10];
			public string this[int index]
			{
				get { return items[index]; }
				set { items[index] = value; }
			}
			public string this[int index, int index2]
			{
				get { return items[index + index2]; }
				set { items[index + index2] = value; }
			}

			#endregion // 实例属性

			#region 静态和实例字段

			public static string TestStaticField = null;
			public string TestInstanceField = null;

			#endregion // 静态和实例字段

		}
	}
}
