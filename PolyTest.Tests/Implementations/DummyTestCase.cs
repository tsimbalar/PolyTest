﻿using System;

namespace PolyTest.Tests.Implementations
{
    public class DummyTestCase<T> : ITestCase<T>
    {
        protected bool Equals(DummyTestCase<T> other)
        {
            return string.Equals(_description, other._description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DummyTestCase<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_description != null ? _description.GetHashCode() : 0);
        }

        private readonly string _description;

        public DummyTestCase(string description)
        {
            _description = description;
        }

        public string Description { get { return _description; } }
        public T Arrange()
        {
            throw new NotImplementedException();
        }
    }
}