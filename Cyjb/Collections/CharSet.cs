using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
    /// <summary>
    /// 表示特定于字符的集合，可以按字母顺序遍历集合。
    /// </summary>
    /// <remarks><see cref="CharSet"/> 类采用类似位示图的树状位压缩数组判断字符是否存在，
    /// 关于该数据结构的更多解释，请参见我的博文
    /// <see href="http://www.cnblogs.com/cyjb/archive/p/CharSet.html">
    /// 《基于树状位压缩数组的字符集合》</see>。</remarks>
    /// <seealso href="http://www.cnblogs.com/cyjb/archive/p/CharSet.html">
    /// 《基于树状位压缩数组的字符集合》</seealso>
    [Serializable]
	public sealed class CharSet : SetBase<char>, ISerializable
	{

		#region 常量定义

		/// <summary>
		/// 顶层数组的长度。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int TopLen = 64;
		/// <summary>
		/// 底层数组的长度。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int BtmLen = 32;
		/// <summary>
		/// 顶层数组索引的位移。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int TopShift = 10;
		/// <summary>
		/// 底层数组索引的位移。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int BtmShift = 5;
		/// <summary>
		/// 底间层数组索引的掩码。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int BtmMask = 0x1F;
		/// <summary>
		/// UInt32 索引的掩码。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IndexMask = 0x1F;

		#endregion // 常量定义

		/// <summary>
		/// 0x0000 ~ 0xFFFF 的数据。
		/// </summary>
		private uint[][] data;
		/// <summary>
		/// 集合中元素的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 底层数组的完整长度。
		/// </summary>
		private readonly int btmFullLen;
		/// <summary>
		/// 集合是否不区分大小写。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool ignoreCase;
		/// <summary>
		/// 判断字符大小写使用的区域信息。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CultureInfo culture;
		/// <summary>
		/// 获取字符索引的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Func<char, int> getIndex;

		#region 构造函数

		/// <summary>
		/// 初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public CharSet()
			: this(false, null)
		{ }
		/// <summary>
		/// 使用指定的是否区分大小写初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		public CharSet(bool ignoreCase)
			: this(ignoreCase, null)
		{ }
		/// <summary>
		/// 使用指定的是否区分大小写和区域信息初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		/// <param name="culture">不区分字符大小写时使用的区域信息。</param>
		public CharSet(bool ignoreCase, CultureInfo culture)
			: base(null)
		{
			this.ignoreCase = ignoreCase;
			data = new uint[TopLen][];
			if (this.ignoreCase)
			{
				this.culture = culture ?? CultureInfo.InvariantCulture;
				getIndex = GetIndexIgnoreCase;
				btmFullLen = BtmLen << 1;
			}
			else
			{
				getIndex = GetIndex;
				btmFullLen = BtmLen;
			}
		}
		/// <summary>
		/// 初始化 <see cref="CharSet"/> 类的新实例，该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public CharSet(IEnumerable<char> collection)
			: this(false, null)
		{
			CommonExceptions.CheckArgumentNull(collection, nameof(collection));
			Contract.EndContractBlock();
			UnionWith(collection);
		}
		/// <summary>
		/// 使用指定的是否区分大小写初始化 <see cref="CharSet"/> 类的新实例，
		/// 该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public CharSet(IEnumerable<char> collection, bool ignoreCase)
			: this(ignoreCase, null)
		{
			CommonExceptions.CheckArgumentNull(collection, nameof(collection));
			Contract.EndContractBlock();
			UnionWith(collection);
		}
		/// <summary>
		/// 使用指定的是否区分大小写和区域信息初始化 
		/// <see cref="CharSet"/> 类的新实例，
		/// 该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		/// <param name="culture">不区分字符大小写时使用的区域信息。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public CharSet(IEnumerable<char> collection, bool ignoreCase, CultureInfo culture)
			: this(ignoreCase, culture)
		{
			CommonExceptions.CheckArgumentNull(collection, nameof(collection));
			Contract.EndContractBlock();
			UnionWith(collection);
		}
		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> 对象，包含序列化 
		/// <see cref="CharSet"/> 所需的信息。</param>
		/// <param name="context"><see cref="StreamingContext"/> 对象，
		/// 该对象包含与 <see cref="CharSet"/> 相关联的序列化流的源和目标。</param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 参数为 <c>null</c>。</exception>
		private CharSet(SerializationInfo info, StreamingContext context)
			: base(null)
		{
			CommonExceptions.CheckArgumentNull(info, nameof(info));
			Contract.EndContractBlock();
			data = (uint[][])info.GetValue("Data", typeof(uint[][]));
			count = info.GetInt32("Count");
			ignoreCase = info.GetBoolean("IgnoreCase");
			if (ignoreCase)
			{
				culture = (CultureInfo)info.GetValue("Culture", typeof(CultureInfo));
				getIndex = GetIndexIgnoreCase;
				btmFullLen = BtmLen << 1;
			}
			else
			{
				getIndex = GetIndex;
				btmFullLen = BtmLen;
			}
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取是否使用不区分大小写的比较。
		/// </summary>
		/// <value>如果使用不区分大小写的比较，则为 <c>true</c>；否则为 <c>false</c>；</value>
		public bool IgnoreCase
		{
			get { return ignoreCase; }
		}
		/// <summary>
		/// 获取不区分大小写时使用的区域性信息。
		/// </summary>
		/// <value>在进行不区分大小写的比较时，使用的区域性信息。</value>
		public CultureInfo Culture
		{
			get { return culture; }
		}
		/// <summary>
		/// 对象不变量。
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(data != null && data.Length == TopLen);
		}

		#region 数据操作

		/// <summary>
		/// 返回指定的底层存储单元是否都是 0。
		/// </summary>
		/// <param name="array">要判断的底层存储单元。</param>
		/// <returns>如果单元中的元素都是 0，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsEmpty(uint[] array)
		{
			Contract.Requires(array != null && array.Length >= BtmLen);
			for (var i = 0; i < BtmLen; i++)
			{
				if (array[i] != 0)
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 计算指定的底层存储单元中包含的字符个数。
		/// </summary>
		/// <param name="array">要计算字符个数的底层存储单元。</param>
		/// <returns>指定的底层存储单元中包含的字符个数。</returns>
		private static int CountChar(uint[] array)
		{
			Contract.Requires(array != null && array.Length >= BtmLen);
			Contract.Ensures(Contract.Result<int>() >= 0);
			var cnt = 0;
			for (var i = 0; i < BtmLen; i++)
			{
				if (array[i] != 0)
				{
					cnt += array[i].CountBits();
				}
			}
			return cnt;
		}
		/// <summary>
		/// 复制指定的底层存储单元
		/// </summary>
		/// <param name="array">要复制底层存储单元。</param>
		/// <param name="count">实际被复制的字符个数。</param>
		/// <returns>复制得到的底层存储单元。。</returns>
		private static uint[] CopyChar(uint[] array, out int count)
		{
			Contract.Requires(array != null && array.Length >= BtmLen);
			Contract.Ensures(Contract.Result<uint[]>() != null && 
				Contract.Result<uint[]>().Length == array.Length);
			Contract.Ensures(Contract.ValueAtReturn(out count) >= 0);
			var newArr = new uint[array.Length];
			array.CopyTo(newArr, 0);
			count = CountChar(array);
			return newArr;
		}
		/// <summary>
		/// 默认的获取字符索引的方法。
		/// </summary>
		/// <param name="ch">要获取索引的字符。</param>
		/// <returns>指定字符的索引。</returns>
		private static int GetIndex(char ch)
		{
			Contract.Ensures(Contract.Result<int>() >= 0);
			return ch;
		}
		/// <summary>
		/// 不区分大小写的获取字符索引的方法。
		/// </summary>
		/// <param name="ch">要获取索引的字符。</param>
		/// <returns>指定字符的索引。</returns>
		private int GetIndexIgnoreCase(char ch)
		{
			Contract.Ensures(Contract.Result<int>() >= 0);
			return char.ToUpper(ch, culture);
		}
		/// <summary>
		/// 获取指定字符对应的数据及相应掩码。如果不存在，则返回 <c>null</c>。
		/// </summary>
		/// <param name="ch">要获取数据及相应掩码的字符。</param>
		/// <param name="idx">数据的索引位置。</param>
		/// <param name="binIdx">数据的二进制位置。</param>
		/// <returns>数据数组。</returns>
		private uint[] FindMask(int ch, out int idx, out uint binIdx)
		{
			Contract.Requires(ch >= 0 && ch >> TopShift < TopLen);
			Contract.Ensures(Contract.Result<uint[]>() == null ||
				Contract.Result<uint[]>().Length == btmFullLen);
			idx = ch >> TopShift;
			var arr = data[idx];
			if (arr == null)
			{
				binIdx = 0;
			}
			else
			{
				idx = (ch >> BtmShift) & BtmMask;
				binIdx = 1U << (ch & IndexMask);
			}
			return arr;
		}
		/// <summary>
		/// 获取指定字符对应的掩码位置。如果掩码位置不存在，则创建指定的掩码位置。
		/// </summary>
		/// <param name="ch">要获取数据及相应掩码的字符。</param>
		/// <param name="idx">数据的索引位置。</param>
		/// <param name="binIdx">数据的二进制位置。</param>
		/// <returns>数据数组。</returns>
		private uint[] FindAndCreateMask(int ch, out int idx, out uint binIdx)
		{
			Contract.Requires(ch >= 0 && ch >> TopShift < TopLen);
			Contract.Ensures(Contract.Result<uint[]>() != null &&
				Contract.Result<uint[]>().Length == btmFullLen);
			Contract.Ensures(Contract.ValueAtReturn(out idx) >= 0 &&
				Contract.ValueAtReturn(out idx) < TopLen);
			idx = ch >> TopShift;
			var arr = data[idx];
			if (arr == null)
			{
				arr = new uint[btmFullLen];
				data[idx] = arr;
			}
			idx = (ch >> BtmShift) & BtmMask;
			binIdx = 1u << (ch & IndexMask);
			return arr;
		}
		/// <summary>
		/// 确定当前集与指定集合相比，相同的和未包含的元素数目。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <param name="returnIfUnfound">是否遇到未包含的元素就返回。</param>
		/// <param name="sameCount">当前集合中相同元素的数目。</param>
		/// <param name="unfoundCount">当前集合中未包含的元素数目。</param>
		private void CountElements(IEnumerable<char> other,
			bool returnIfUnfound, out int sameCount, out int unfoundCount)
		{
			Contract.Requires(other != null);
			Contract.Ensures(Contract.ValueAtReturn(out sameCount) >= 0);
			Contract.Ensures(Contract.ValueAtReturn(out unfoundCount) >= 0);
			sameCount = unfoundCount = 0;
			var uniqueSet = new CharSet(ignoreCase, culture);
			foreach (var ch in other)
			{
				if (Contains(ch))
				{
					if (uniqueSet.Add(ch))
					{
						sameCount++;
					}
				}
				else
				{
					unfoundCount++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
		}
		/// <summary>
		/// 确定当前集是否包含指定的集合中的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果包含 <paramref name="other"/> 中的所有元素，则返回 
		/// <c>true</c>，否则返回 <c>false</c>。</returns>
		private bool ContainsAllElements(CharSet other)
		{
			Contract.Requires(other != null);
			for (var i = 0; i < TopLen; i++)
			{
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					continue;
				}
				var arr = data[i];
				if (arr == null)
				{
					if (!IsEmpty(otherArr))
					{
						return false;
					}
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					if ((arr[j] | otherArr[j]) != arr[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		#endregion

		/// <summary>
		/// 将 <see cref="CharSet"/> 对象的容量设置为它所包含
		/// 的元素的实际个数，向上舍入为接近的特定于实现的值。
		/// </summary>
		public void TrimExcess()
		{
			for (var i = 0; i < TopLen; i++)
			{
				if (data[i] != null && IsEmpty(data[i]))
				{
					data[i] = null;
				}
			}
		}

		#region SetBase<char> 成员

		/// <summary>
		/// 向当前集内添加元素，并返回一个指示是否已成功添加元素的值。
		/// </summary>
		/// <param name="item">要添加到 <see cref="CharSet"/> 的中的对象。
		/// 对于引用类型，该值可以为 <c>null</c>。</param>
		/// <returns>如果该元素已添加到集内，则为 <c>true</c>；
		/// 如果该元素已在集内，则为 <c>false</c>。</returns>
		protected override bool AddItem(char item)
		{
			var cIdx = getIndex(item);
			int idx;
			uint binIdx;
			var arr = FindAndCreateMask(cIdx, out idx, out binIdx);
			if ((arr[idx] & binIdx) != 0U)
			{
				return false;
			}
			arr[idx] |= binIdx;
			count++;
			if (ignoreCase && cIdx != item)
			{
				// 添加的是小写字母。
				arr[idx + BtmLen] |= binIdx;
			}
			return true;
		}

		#endregion // SetBase<char> 成员

		#region ISet<char> 成员

		/// <summary>
		/// 从当前集内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集内移除的项的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void ExceptWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count <= 0)
			{
				return;
			}
			if (ReferenceEquals(this, other))
			{
				Clear();
			}
			else
			{
				var otherSet = other as CharSet;
				if (otherSet != null &&
					ignoreCase == otherSet.ignoreCase &&
					Equals(culture, otherSet.culture))
				{
					// 针对 CharSet 的操作更快。
					ExceptWith(otherSet);
				}
				else
				{
					foreach (var c in other)
					{
						Remove(c);
					}
				}
			}
		}
		/// <summary>
		/// 从当前集内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集内移除的项的集合。</param>
		private void ExceptWith(CharSet other)
		{
			Contract.Requires(other != null);
			for (var i = 0; i < TopLen; i++)
			{
				var arr = data[i];
				if (arr == null)
				{
					continue;
				}
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					var removed = arr[j] & otherArr[j];
					if (removed > 0U)
					{
						count -= removed.CountBits();
						arr[j] &= ~removed;
						if (ignoreCase)
						{
							arr[j + BtmLen] &= ~removed;
						}
					}
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void IntersectWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count <= 0 || ReferenceEquals(this, other))
			{
				return;
			}
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				// 针对 CharSet 的操作更快。
				IntersectWith(otherSet);
			}
			else
			{
				otherSet = new CharSet(other.Where(Contains), ignoreCase, culture);
				// 替换当前集合。
				data = otherSet.data;
				count = otherSet.count;
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void IntersectWith(CharSet other)
		{
			Contract.Requires(other != null);
			// 针对 CharSet 的操作更快。
			for (var i = 0; i < TopLen; i++)
			{
				var arr = data[i];
				if (arr == null)
				{
					continue;
				}
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					data[i] = null;
					// 计算被移除的元素数量。
					count -= CountChar(arr);
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					var removed = arr[j] & ~otherArr[j];
					if (removed > 0U)
					{
						count -= removed.CountBits();
						arr[j] &= otherArr[j];
						if (ignoreCase)
						{
							arr[j + BtmLen] &= otherArr[j];
						}
					}
				}
			}
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真（严格）子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真子集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsProperSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			var col = other as ICollection<char>;
			if (count == 0)
			{
				if (col == null)
				{
					return other.Any();
				}
				return col.Count > 0;
			}
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				return count < otherSet.count && otherSet.ContainsAllElements(this);
			}
			int sameCount, unfoundCount;
			CountElements(other, false, out sameCount, out unfoundCount);
			return sameCount == count && unfoundCount > 0;
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真（严格）超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真超集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsProperSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count == 0)
			{
				return false;
			}
			var col = other as ICollection<char>;
			if (col != null && col.Count == 0)
			{
				return true;
			}
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				return otherSet.count < count && ContainsAllElements(otherSet);
			}
			int sameCount, unfoundCount;
			CountElements(other, true, out sameCount, out unfoundCount);
			return sameCount < Count && unfoundCount == 0;
		}
		/// <summary>
		/// 确定当前集是否为指定集合的子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的子集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count == 0)
			{
				return true;
			}
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				return count <= otherSet.Count && otherSet.ContainsAllElements(this);
			}
			int sameCount, unfoundCount;
			CountElements(other, false, out sameCount, out unfoundCount);
			return sameCount == count && unfoundCount >= 0;
		}
		/// <summary>
		/// 确定当前集是否为指定集合的超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的超集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			var col = other as ICollection<char>;
			if (col != null)
			{
				if (col.Count == 0)
				{
					return true;
				}
				if (count == 0)
				{
					return false;
				}
			}
			var otherSet = other as CharSet;
			if (otherSet == null ||
				ignoreCase != otherSet.ignoreCase ||
				!Equals(culture, otherSet.culture))
			{
				return other.All(Contains);
			}
			return otherSet.Count <= count && ContainsAllElements(otherSet);
		}
		/// <summary>
		/// 确定当前集是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool Overlaps(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count <= 0)
			{
				return false;
			}
			var otherSet = other as CharSet;
			if (otherSet == null ||
				ignoreCase != otherSet.ignoreCase ||
				!Equals(culture, otherSet.culture))
			{
				return other.Any(Contains);
			}
			// 针对 CharSet 的操作更快。
			return Overlaps(otherSet);
		}
		/// <summary>
		/// 确定当前集是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool Overlaps(CharSet other)
		{
			Contract.Requires(other != null);
			for (var i = 0; i < TopLen; i++)
			{
				var arr = data[i];
				if (arr == null)
				{
					continue;
				}
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					if ((arr[j] & otherArr[j]) > 0U)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// 确定当前集与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool SetEquals(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				return count == otherSet.count && ContainsAllElements(otherSet);
			}
			var col = other as ICollection<char>;
			if (count == 0)
			{
				if (col == null)
				{
					return !other.Any();
				}
				if (col.Count > 0)
				{
					return false;
				}
			}
			int sameCount, unfoundCount;
			CountElements(other, true, out sameCount, out unfoundCount);
			return sameCount == count && unfoundCount == 0;
		}
		/// <summary>
		/// 修改当前集，使该集仅包含当前集或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void SymmetricExceptWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (count == 0)
			{
				UnionWith(other);
			}
			else if (ReferenceEquals(this, other))
			{
				Clear();
			}
			else
			{
				var otherSet = other as CharSet;
				if (otherSet == null ||
					ignoreCase != otherSet.ignoreCase ||
					!Equals(culture, otherSet.culture))
				{
					otherSet = new CharSet(other, ignoreCase, culture);
				}
				// 针对 CharSet 的操作更快。
				SymmetricExceptWith(otherSet);
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含当前集或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void SymmetricExceptWith(CharSet other)
		{
			Contract.Requires(other != null);
			for (var i = 0; i < TopLen; i++)
			{
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					continue;
				}
				var arr = data[i];
				if (arr == null)
				{
					// 复制数据。
					int cnt;
					data[i] = CopyChar(otherArr, out cnt);
					count += cnt;
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					var oldCnt = 0;
					if (arr[j] > 0)
					{
						oldCnt = arr[j].CountBits();
					}
					if (ignoreCase)
					{
						arr[j + BtmLen] &= ~otherArr[j];
						arr[j + BtmLen] |= otherArr[j + BtmLen] & ~arr[j];
					}
					arr[j] ^= otherArr[j];
					var newCnt = 0;
					if (arr[j] > 0)
					{
						newCnt = arr[j].CountBits();
					}
					count += newCnt - oldCnt;
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集包含当前集和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void UnionWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other, nameof(other));
			Contract.EndContractBlock();
			if (ReferenceEquals(this, other))
			{
				return;
			}
			var otherSet = other as CharSet;
			if (otherSet != null &&
				ignoreCase == otherSet.ignoreCase &&
				Equals(culture, otherSet.culture))
			{
				// 针对 CharSet 的操作更快。
				UnionWith(otherSet);
			}
			else
			{
				foreach (var c in other)
				{
					AddItem(c);
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集包含当前集和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void UnionWith(CharSet other)
		{
			Contract.Requires(other != null);
			for (var i = 0; i < TopLen; i++)
			{
				var otherArr = other.data[i];
				if (otherArr == null)
				{
					continue;
				}
				var arr = data[i];
				if (arr == null)
				{
					// 复制数据。
					int cnt;
					data[i] = CopyChar(otherArr, out cnt);
					count += cnt;
					continue;
				}
				for (var j = 0; j < BtmLen; j++)
				{
					// 后来添加的字符数。
					var added = ~arr[j] & otherArr[j];
					if (added > 0)
					{
						count += added.CountBits();
						arr[j] |= added;
						if (ignoreCase)
						{
							arr[j + BtmLen] |= otherArr[j + BtmLen] & added;
						}
					}
				}
			}
		}

		#endregion // ISet<char> 成员

		#region ICollection<char> 成员

		/// <summary>
		/// 获取 <see cref="CharSet"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="CharSet"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return count; }
		}
		/// <summary>
		/// 从 <see cref="CharSet"/> 中移除所有元素。
		/// </summary>
		public override void Clear()
		{
			count = 0;
			for (var i = 0; i < TopLen; i++)
			{
				data[i] = null;
			}
		}
		/// <summary>
		/// 确定 <see cref="CharSet"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="CharSet"/> 
		/// 中定位的对象。</param>
		/// <returns>如果在 <see cref="CharSet"/> 
		/// 中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(char item)
		{
			int idx;
			uint binIdx;
			var arr = FindMask(getIndex(item), out idx, out binIdx);
			return (arr != null) && ((arr[idx] & binIdx) != 0U);
		}
		/// <summary>
		/// 从 <see cref="CharSet"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="CharSet"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="CharSet"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="CharSet"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		public override bool Remove(char item)
		{
			var cIdx = getIndex(item);
			int idx;
			uint binIdx;
			var arr = FindMask(cIdx, out idx, out binIdx);
			if (arr == null || (arr[idx] & binIdx) == 0U)
			{
				return false;
			}
			arr[idx] &= ~binIdx;
			count--;
			if (ignoreCase && cIdx != item)
			{
				// 删除的是小写字母。
				arr[idx + BtmLen] &= ~binIdx;
			}
			return true;
		}

		#endregion // ICollection<char> 成员

		#region IEnumerable<char> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/>。</returns>
		public override IEnumerator<char> GetEnumerator()
		{
			for (var i = 0; i < TopLen; i++)
			{
				var arr = data[i];
				if (arr == null)
				{
					continue;
				}
				var highPart = i << TopShift;
				for (var k = 0; k < BtmLen; k++)
				{
					var midPart = highPart | (k << BtmShift);
					var value = arr[k];
					var ignoreCaseFlags = ignoreCase ? arr[k + BtmLen] : 0U;
					for (var n = -1; value > 0U; )
					{
						var oneIdx = (value & 1U) == 1U ? 1 : value.CountTrailingZeroBits() + 1;
						if (oneIdx == 32)
						{
							// C# 中 uint 右移 32 位会不变。
							value = 0U;
						}
						else
						{
							value = value >> oneIdx;
						}
						n += oneIdx;
						var lowPart = (char)(midPart | n);
						if ((ignoreCaseFlags & (1U << n)) > 0U)
						{
							lowPart = char.ToLower(lowPart, culture);
						}
						yield return lowPart;
					}
				}
			}
		}

		#endregion // IEnumerable<char> 成员

		#region ISerializable 成员

		/// <summary>
		/// 使用将目标对象序列化所需的数据填充 <see cref="SerializationInfo"/>。
		/// </summary>
		/// <param name="info">要填充数据的 <see cref="SerializationInfo"/>。
		/// </param>
		/// <param name="context">此序列化的目标。</param>
		/// <exception cref="SecurityException">调用方没有所要求的权限。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 参数为 <c>null</c>。</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			CommonExceptions.CheckArgumentNull(info, nameof(info));
			Contract.EndContractBlock();
			info.AddValue("Data", data);
			info.AddValue("Count", count);
			info.AddValue("IgnoreCase", ignoreCase);
			if (ignoreCase)
			{
				info.AddValue("Culture", culture);
			}
		}

		#endregion

	}
}