﻿//
// Copyright (c) 2009-2018 Krueger Systems, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591 // XML Doc Comments

namespace SQLite
{
	public class SQLiteException : Exception
	{
		public SQLite3.Result Result { get; private set; }

		protected SQLiteException(SQLite3.Result r, string message) : base(message)
		{
			Result = r;
		}

		public static SQLiteException New(SQLite3.Result r, string message)
		{
			return new SQLiteException(r, message);
		}
	}

	public class NotNullConstraintViolationException : SQLiteException
	{
		public IEnumerable<TableMapping.Column>? Columns { get; protected set; }

		protected NotNullConstraintViolationException(SQLite3.Result r, string message)
			: this(r, message, null, null)
		{

		}

		protected NotNullConstraintViolationException(SQLite3.Result r, string message, TableMapping? mapping, object? obj)
			: base(r, message)
		{
			if(mapping != null && obj != null) {
				Columns = from c in mapping.Columns
						  where c.IsNullable == false && c.GetProperty(obj!) == null
						  select c;
			}
		}

		public static new NotNullConstraintViolationException New(SQLite3.Result r, string message)
		{
			return new NotNullConstraintViolationException(r, message);
		}

		public static NotNullConstraintViolationException New(SQLite3.Result r, string message, TableMapping mapping, object obj)
		{
			return new NotNullConstraintViolationException(r, message, mapping, obj);
		}

		public static NotNullConstraintViolationException New(SQLiteException exception, TableMapping mapping, object obj)
		{
			return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
		}
	}
}
