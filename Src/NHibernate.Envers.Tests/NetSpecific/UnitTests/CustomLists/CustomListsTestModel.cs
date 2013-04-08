﻿
using System;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists
{
	internal interface IAuditParent
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string Value { get; set; }
		ICustomList<IAuditChild> Children { get; }
	}

	internal class AuditParent : IAuditParent
	{
		private Guid _id;
		private string _name;
		private string _value;
		private AutomaticCustomCollection<IAuditChild> _children;

		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ICustomList<IAuditChild> Children
		{
			get { return _children; }
		}

		private ICustomList<IAuditChild> ChildrenInternal
		{
			get
			{
				return _children.List;
			}
			set
			{
				_children.List = value;
			}
		}
	}

	internal interface IAuditChild
	{
		Guid Id { get; set; }
		string ChildName { get; set; }
		int ChildValue { get; set; }
	}

	class AuditChild : IAuditChild
	{
		private Guid _id;
		private string _childName;
		private int _childValue;

		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string ChildName
		{
			get { return _childName; }
			set { _childName = value; }
		}

		public int ChildValue
		{
			get { return _childValue; }
			set { _childValue = value; }
		}
	}
}
