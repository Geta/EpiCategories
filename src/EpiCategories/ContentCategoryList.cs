using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Entity;

namespace Geta.EpiCategories
{
    public class ContentCategoryList : IComparable, IList<ContentReference>, IReadOnly<ContentCategoryList>, IReadOnlyCollection<ContentReference>, IModifiedTrackable, IEquatable<ContentCategoryList>, IEnumerable<string>
    {
        private bool _isModified;
        private List<ContentReference> _innerList;

        public ContentCategoryList()
        {
            _innerList = new List<ContentReference>();
        }

        public ContentCategoryList(IEnumerable<ContentReference> categories)
        {
            _innerList = new List<ContentReference>(categories);
        }

        public int Count
        {
            get { return _innerList.Count; }
        }

        public virtual bool IsEmpty
        {
            get { return Count == 0; }
        }

        public virtual bool IsModified
        {
            get { return _isModified; }
            set
            {
                ThrowIfReadOnly();
                _isModified = value;
            }
        }

        public virtual bool IsReadOnly { get; private set; }

        public static bool operator ==(ContentCategoryList x, ContentCategoryList y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return x.Equals(y);
        }

        public static bool operator !=(ContentCategoryList x, ContentCategoryList y)
        {
            return x == y == false;
        }

        public void Add(ContentReference item)
        {
            ThrowIfReadOnly();

            if (_innerList.Contains(item))
            {
                return;
            }

            _innerList.Add(item);
            _isModified = true;
        }

        public void Clear()
        {
            ThrowIfReadOnly();
            _innerList.Clear();
            _isModified = true;
        }

        public int CompareTo(object x)
        {
            if (this.GetType() != x.GetType())
                throw new ArgumentException("Object not of the same type");

            if (this == (ContentCategoryList) x)
                return 0;

            return this.Count > ((ContentCategoryList) x).Count ? 1 : -1;
        }

        public bool Contains(ContentReference item)
        {
            return _innerList.Any(x => x.CompareToIgnoreWorkID(item));
        }

        public ContentCategoryList Copy()
        {
            var categoryList = (ContentCategoryList)MemberwiseClone();
            var list = new List<ContentReference>(_innerList);
            categoryList._innerList = list;
            return categoryList;
        }

        public void CopyTo(ContentReference[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public ContentCategoryList CreateWritableClone()
        {
            var categoryList = Copy();
            categoryList.IsReadOnly = false;
            return categoryList;
        }

        object IReadOnly.CreateWritableClone()
        {
            return CreateWritableClone();
        }

        public bool Equals(ContentCategoryList other)
        {
            if (other == null)
                return false;

            if (IsEmpty && other.IsEmpty)
                return true;

            if (Count == other.Count)
                return other.MemberOfAll(_innerList);

            return false;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return _innerList.Select(x => x.ToReferenceWithoutVersion().ToString()).GetEnumerator();
        }

        public IEnumerator<ContentReference> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return _innerList.GetHashCode();
        }

        public int IndexOf(ContentReference item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, ContentReference item)
        {
            ThrowIfReadOnly();
            _innerList.Insert(index, item);
            _isModified = true;
        }

        public void MakeReadOnly()
        {
            if (IsReadOnly)
                return;

            IsModified = false;
            IsReadOnly = true;
        }

        public virtual bool MemberOf(ContentReference category)
        {
            return Contains(category);
        }

        public virtual bool MemberOfAny(IEnumerable<ContentReference> categories)
        {
            if (categories == null || categories.Any() == false)
            {
                return true;
            }

            return categories.Any(Contains);
        }

        public virtual bool MemberOfAll(IEnumerable<ContentReference> categories)
        {
            if (categories == null || categories.Any() == false)
            {
                return true;
            }

            return categories.All(Contains);
        }

        public bool Remove(ContentReference item)
        {
            ThrowIfReadOnly();

            if (Contains(item))
            {
                return false;
            }

            _isModified = true;
            return _innerList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();
            _innerList.RemoveAt(index);
            _isModified = true;
        }

        public void ResetModified()
        {
            IsModified = false;
        }

        public ContentReference this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                ThrowIfReadOnly();
                _isModified = true;
                _innerList[index] = value;
            }
        }

        protected void ThrowIfReadOnly()
        {
            Validator.ValidateNotReadOnly(this);
        }
    }
}