using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.TextValuePair{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTextValuePair
	{
		/// <summary>
		/// 测试 <see cref="TextValuePair{TValue}"/> 的构造函数，
		/// <see cref="TextValuePair{T}.Text"/> 属性和
		/// <see cref="TextValuePair{T}.Value"/> 属性。
		/// </summary>
		[TestMethod]
		public void TestProperty()
		{
			var target = new TextValuePair<GenericParameterHelper>();
			Assert.AreEqual(target.Text, default(string), "测试 Text 默认值失败。");
			Assert.AreEqual(target.Value, default(GenericParameterHelper), "测试 Value 默认值失败。");
			target.Text = "TT";
			target.Value = new GenericParameterHelper(1);
			Assert.AreEqual(target.Text, "TT", "测试 Text 赋值失败。");
			Assert.AreEqual(target.Value, new GenericParameterHelper(1), "测试 Value 赋值失败。");
			target = new TextValuePair<GenericParameterHelper>("TT",
				new GenericParameterHelper(2));
			Assert.AreEqual(target.Text, "TT", "测试 Text 构造函数赋值失败。");
			Assert.AreEqual(target.Value, new GenericParameterHelper(2), "测试 Value 构造函数赋值失败。");
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 对 
		/// <see cref="TextValuePair{T}.Text"/> 属性改变的事件。
		/// </summary>
		[TestMethod]
		public void TestTextEvent()
		{
			var target = new TextValuePair<GenericParameterHelper>("AA", null);
			var changing = false;
			var changed = false;
			var error = false;
			target.PropertyChanging += (sender, e) =>
			{
				if (e.PropertyName == "Text")
				{
					changing = true;
					if (target.Text != "AA")
					{
						error = true;
					}
				}
			};
			target.PropertyChanged += (sender, e) =>
			{
				if (changing && e.PropertyName == "Text")
				{
					changed = true;
					if (target.Text != "BB")
					{
						error = true;
					}
				}
			};
			target.Text = "BB";
			Assert.IsFalse(error);
			Assert.IsTrue(changing);
			Assert.IsTrue(changed);
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 对 
		/// <see cref="TextValuePair{T}.Value"/> 属性改变的事件。
		/// </summary>
		[TestMethod]
		public void TestValueEvent()
		{
			var para = new GenericParameterHelper();
			var target =
				new TextValuePair<GenericParameterHelper>("AA", null);
			var changing = false;
			var changed = false;
			var error = false;
			target.PropertyChanging += (sender, e) =>
			{
				if (e.PropertyName == "Value")
				{
					changing = true;
					if (target.Value != null)
					{
						error = true;
					}
				}
			};
			target.PropertyChanged += (sender, e) =>
			{
				if (changing && e.PropertyName == "Value")
				{
					changed = true;
					if (target.Value != para)
					{
						error = true;
					}
				}
			};
			target.Value = para;
			Assert.IsFalse(error);
			Assert.IsTrue(changing);
			Assert.IsTrue(changed);
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 的相等比较。
		/// </summary>
		[TestMethod]
		public void TestEqual()
		{
			var value = new GenericParameterHelper();
			var pair1 =
				new TextValuePair<GenericParameterHelper>();
			var pair2 =
				new TextValuePair<GenericParameterHelper>();
			Assert.IsTrue(pair1.Equals(pair2));
			Assert.IsTrue(pair1 == pair2);
			Assert.IsFalse(pair1 != pair2);
			pair1.Text = "1";
			Assert.IsFalse(pair1.Equals(pair2));
			Assert.IsFalse(pair1 == pair2);
			Assert.IsTrue(pair1 != pair2);
			pair2.Text = "12".Substring(0, 1);
			Assert.IsTrue(pair1.Equals(pair2));
			Assert.IsTrue(pair1 == pair2);
			Assert.IsFalse(pair1 != pair2);
			pair1.Value = value;
			Assert.IsFalse(pair1.Equals(pair2));
			Assert.IsFalse(pair1 == pair2);
			Assert.IsTrue(pair1 != pair2);
			pair2.Value = value;
			Assert.IsTrue(pair1.Equals(pair2));
			Assert.IsTrue(pair1 == pair2);
			Assert.IsFalse(pair1 != pair2);
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 的 
		/// <see cref="TextValuePair{T}.Text"/> 属性对 
		/// <see cref="IEditableObject"/> 接口的实现。
		/// </summary>
		[TestMethod]
		public void TestEditableObjectText()
		{
			var target =
				new TextValuePair<GenericParameterHelper>("AA", null);
			var obj = (IEditableObject)target;
			var changing = false;
			var changed = false;
			var error = false;
			target.PropertyChanging += (sender, e) =>
			{
				if (e.PropertyName == "Text")
				{
					changing = true;
					if (target.Text != "AA")
					{
						error = true;
					}
				}
			};
			target.PropertyChanged += (sender, e) =>
			{
				if (changing && e.PropertyName == "Text")
				{
					changed = true;
					if (target.Text != "BB")
					{
						error = true;
					}
				}
			};
			// Test Text
			obj.BeginEdit();
			target.Text = "BB";
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.EndEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.BeginEdit();
			target.Text = "BB";
			obj.EndEdit();
			Assert.AreEqual(target.Text, "BB");
			Assert.IsFalse(error);
			Assert.IsTrue(changing);
			Assert.IsTrue(changed);
			changing = false;
			changed = false;
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "BB");
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			// Test Text without change
			changing = changed = error = false;
			obj.BeginEdit();
			target.Text = "BB";
			obj.EndEdit();
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 的 
		/// <see cref="TextValuePair{T}.Value"/> 属性对 
		/// <see cref="IEditableObject"/> 接口的实现。
		/// </summary>
		[TestMethod]
		public void TestEditableObjectValue()
		{
			var para1 = new GenericParameterHelper(1);
			var para2 = new GenericParameterHelper(2);
			var target =
				new TextValuePair<GenericParameterHelper>("", para1);
			var obj = (IEditableObject)target;
			var changing = false;
			var changed = false;
			var error = false;
			target.PropertyChanging += (sender, e) =>
			{
				if (e.PropertyName == "Value")
				{
					changing = true;
					if (target.Value != para1)
					{
						error = true;
					}
				}
			};
			target.PropertyChanged += (sender, e) =>
			{
				if (changing && e.PropertyName == "Value")
				{
					changed = true;
					if (target.Value != para2)
					{
						error = true;
					}
				}
			};
			// Test Value
			obj.BeginEdit();
			target.Value = para2;
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.EndEdit();
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.BeginEdit();
			target.Value = para2;
			obj.EndEdit();
			Assert.AreEqual(target.Value, para2);
			Assert.IsFalse(error);
			Assert.IsTrue(changing);
			Assert.IsTrue(changed);
			changing = false;
			changed = false;
			obj.CancelEdit();
			Assert.AreEqual(target.Value, para2);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			// Test Value without change
			changing = changed = error = false;
			obj.BeginEdit();
			target.Value = para2;
			obj.EndEdit();
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
		}
		/// <summary>
		/// 测试 <see cref="TextValuePair{T}"/> 的 
		/// <see cref="TextValuePair{T}.Text"/> 和 
		/// <see cref="TextValuePair{T}.Value"/> 属性对 
		/// <see cref="IEditableObject"/> 接口的实现。
		/// </summary>
		[TestMethod]
		public void TestEditableObjectTextAndValue()
		{
			var para1 = new GenericParameterHelper(1);
			var para2 = new GenericParameterHelper(2);
			var target =
				new TextValuePair<GenericParameterHelper>("AA", para1);
			var obj = (IEditableObject)target;
			var changing = false;
			var changed = false;
			var error = false;
			target.PropertyChanging += (sender, e) =>
			{
				if (e.PropertyName == string.Empty)
				{
					changing = true;
					if (target.Text != "AA" || target.Value != para1)
					{
						error = true;
					}
				}
			};
			target.PropertyChanged += (sender, e) =>
			{
				if (changing && e.PropertyName == string.Empty)
				{
					changed = true;
					if (target.Text != "BB" || target.Value != para2)
					{
						error = true;
					}
				}
			};
			// Test Text And Value
			obj.BeginEdit();
			target.Text = "BB";
			target.Value = para2;
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.EndEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "AA");
			Assert.AreEqual(target.Value, para1);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			obj.BeginEdit();
			target.Text = "BB";
			target.Value = para2;
			obj.EndEdit();
			Assert.AreEqual(target.Text, "BB");
			Assert.AreEqual(target.Value, para2);
			Assert.IsFalse(error);
			Assert.IsTrue(changing);
			Assert.IsTrue(changed);
			changing = false;
			changed = false;
			obj.CancelEdit();
			Assert.AreEqual(target.Text, "BB");
			Assert.AreEqual(target.Value, para2);
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
			// Test Text And Value without change
			changing = changed = error = false;
			obj.BeginEdit();
			target.Text = "BB";
			target.Value = para2;
			obj.EndEdit();
			Assert.IsFalse(error);
			Assert.IsFalse(changing);
			Assert.IsFalse(changed);
		}
	}
}
