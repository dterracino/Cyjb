﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyjb
{
	public static partial class IntegerExt
	{

		#region 常量定义

		/// <summary>
		/// 用于计算以 2 为底的对数值的数组。
		/// </summary>
		private static readonly int[] logBase2_64 =
		{ 
			63,  0, 58,  1, 59, 47, 53,  2, 60, 39, 48, 27, 54, 33, 42,  3,
			61, 51, 37, 40, 49, 18, 28, 20, 55, 30, 34, 11, 43, 14, 22,  4,
			62, 57, 46, 52, 38, 26, 32, 41, 50, 36, 17, 19, 29, 10, 13, 21,
			56, 45, 25, 31, 35, 16,  9, 12, 44, 24, 15,  8, 23,  7,  6,  5
		};

		#endregion // 常量定义

		#region 位运算

		/// <summary>
		/// 判断指定整数是否是 <c>2</c> 的幂。
		/// </summary>
		/// <param name="value">要判断是否是 <c>2</c> 的幂的整数。</param>
		/// <returns>如果指定整数是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>对于 <c>0</c>，结果为 <c>false</c>。</remarks>
		[CLSCompliant(false)]
		public static bool IsPowerOf2(this ulong value)
		{
			return value != 0UL && (value & (value - 1UL)) == 0UL;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中 <c>1</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountBits(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			value -= (value >> 1) & 0x5555555555555555UL;
			value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
			value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
			return (int)((value * 0x0101010101010101UL) >> 56);
		}
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的整数。</param>
		/// <returns>如果指定整数的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；
		/// 否则为 <c>0</c>。</returns>
		[CLSCompliant(false)]
		public static int Parity(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() == 0 || Contract.Result<int>() == 1);
			value ^= value >> 1;
			value ^= value >> 2;
			value = (value & 0x1111111111111111UL) * 0x1111111111111111UL;
			return (int)((value >> 60) & 1);
		}
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的整数。</param>
		/// <returns>指定整数二进制位逆序的结果。</returns>
		[CLSCompliant(false)]
		public static ulong ReverseBits(this ulong value)
		{
			value = ((value >> 1) & 0x5555555555555555UL) | ((value & 0x5555555555555555UL) << 1);
			value = ((value >> 2) & 0x3333333333333333UL) | ((value & 0x3333333333333333UL) << 2);
			value = ((value >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((value & 0x0F0F0F0F0F0F0F0FUL) << 4);
			value = ((value >> 8) & 0x00FF00FF00FF00FFUL) | ((value & 0x00FF00FF00FF00FFUL) << 8);
			value = ((value >> 16) & 0x0000FFFF0000FFFFUL) | ((value & 0x0000FFFF0000FFFFUL) << 16);
			value = (value >> 32) | (value << 32);
			return value;
		}
		/// <summary>
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>2</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>0</c>，则结果未知。</returns>
		[CLSCompliant(false)]
		public static int LogBase2(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			return logBase2_64[((value - (value >> 1)) * 0x07EDD5E59A4E28C2UL) >> 58];
		}
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>10</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>10</c>，则结果未知。</returns>
		[CLSCompliant(false)]
		public static int LogBase10(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 19);
			if (value >= 10000000000000000000UL) { return 19; }
			if (value >= 1000000000000000000UL) { return 18; }
			if (value >= 100000000000000000UL) { return 17; }
			if (value >= 10000000000000000UL) { return 16; }
			if (value >= 1000000000000000UL) { return 15; }
			if (value >= 100000000000000UL) { return 14; }
			if (value >= 10000000000000UL) { return 13; }
			if (value >= 1000000000000UL) { return 12; }
			if (value >= 100000000000UL) { return 11; }
			if (value >= 10000000000UL) { return 10; }
			if (value >= 1000000000UL) { return 9; }
			if (value >= 100000000UL) { return 8; }
			if (value >= 10000000UL) { return 7; }
			if (value >= 1000000UL) { return 6; }
			if (value >= 100000UL) { return 5; }
			if (value >= 10000UL) { return 4; }
			if (value >= 1000UL) { return 3; }
			if (value >= 100UL) { return 2; }
			return value >= 10UL ? 1 : 0;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountTrailingZeroBits(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			return CountTrailingZeroBits((long)value);
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountTrailingBits(this ulong value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			return ((value ^ (value + 1)) >> 1).CountBits();
		}
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于指定整数的最小的 <c>2</c> 的幂次。</returns>
		[CLSCompliant(false)]
		public static ulong CeilingPowerOf2(this ulong value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			value++;
			return value == 0UL ? 1UL : value;
		}
		/// <summary>
		/// 返回指定整数二进制表示的下一字典序排列。
		/// </summary>
		/// <param name="value">要获取下一字典序排列的整数。</param>
		/// <returns>指定整数二进制表示的下一字典序排列。</returns>
		/// <remarks>
		/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
		/// 而且是字典序的下一排列。</para>
		/// <para>例如，<c>102</c> 的二进制表示为 <c>1100110</c>，它的下一字典序排列为
		/// <c>1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
		/// <c>105</c> 的下一字典序排列为 <c>106</c>（二进制为 <c>1101010</c>），
		/// 接下来是 <c>108</c>（二进制为 <c>1101100</c>）和 <c>113</c>（二进制为 <c>1110001</c>）。
		/// </para></remarks>
		[CLSCompliant(false)]
		public static ulong NextBitPermutation(this ulong value)
		{
			var t = value | (value - 1);
			return (t + 1UL) | (((~t & (ulong)-(long)~t) - 1UL) >> (value.CountTrailingZeroBits() + 1));
		}

		#endregion // 位运算

	}
}
