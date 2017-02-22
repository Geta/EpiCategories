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
        protected List<ContentReference> InnerList;

        public ContentCategoryList()
        {
            InnerList = new List<ContentReference>();
        }

        public ContentCategoryList(IEnumerable<ContentReference> categories)
        {
            InnerList = new List<ContentReference>(categories);
        }

        public int Count
        {
            get { return InnerList.Count; }
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

        public void Add(ContentReference item)
        {
            ThrowIfReadOnly();

            if (InnerList.Contains(item))
            {
                return;
            }

            InnerList.Add(item);
            _isModified = true;
        }

        public void Clear()
        {
            ThrowIfReadOnly();
            InnerList.Clear();
            _isModified = true;
        }

        public int CompareTo(object x)
        {
            if (GetType() != x.GetType())
                throw new ArgumentException("Object not of the same type");

            if (this == (ContentCategoryList) x)
                return 0;

            return Count > ((ContentCategoryList) x).Count ? 1 : -1;
        }

        public bool Contains(ContentReference item)
        {
            return InnerList.Any(x => x.CompareToIgnoreWorkID(item));
        }

        public ContentCategoryList Copy()
        {
            var categoryList = (ContentCategoryList)MemberwiseClone();
            var list = new List<ContentReference>(InnerList);
            categoryList.InnerList = list;
            return categoryList;
        }

        public void CopyTo(ContentReference[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
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
                return other.MemberOfAll(InnerList);

            return false;
        }

        public override bool Equals(object other)
        {
            return Equals((ContentCategoryList)other);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return InnerList.Select(x => x.ToReferenceWithoutVersion().ToString()).GetEnumerator();
        }

        public IEnumerator<ContentReference> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return InnerList.GetHashCode();
        }

        public int IndexOf(ContentReference item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, ContentReference item)
        {
            ThrowIfReadOnly();
            InnerList.Insert(index, item);
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
            return categories.Any(Contains);
        }

        public virtual bool MemberOfAll(IEnumerable<ContentReference> categories)
        {
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
            return InnerList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();
            InnerList.RemoveAt(index);
            _isModified = true;
        }

        public void ResetModified()
        {
            IsModified = false;
        }

        public ContentReference this[int index]
        {
            get { return InnerList[index]; }
            set
            {
                ThrowIfReadOnly();
                _isModified = true;
                InnerList[index] = value;
            }
        }

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

        protected void ThrowIfReadOnly()
        {
            Validator.ValidateNotReadOnly(this);
        }
    }
}