using System;
using System.Collections;
using System.Collections.Generic;

namespace SICCSharp.Extensions
{
    /// <summary>
    ///     This extension class is used to chunk enumerable into multiple sub-enumerable and is largely based on a
    ///     Stack-overflow post.
    /// </summary>
    /// <seealso href="https://stackoverflow.com/questions/419019/split-list-into-sublists-with-linq" />
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize < 1) throw new InvalidOperationException();

            var wrapper = new EnumeratorWrapper<T>(source);

            var currentPos = 0;
            try
            {
                wrapper.AddRef();
                while (wrapper.Get(currentPos, out _))
                {
                    yield return new ChunkedEnumerable<T>(wrapper, chunkSize, currentPos);
                    currentPos += chunkSize;
                }
            }
            finally
            {
                wrapper.RemoveRef();
            }
        }

        private class ChunkedEnumerable<T> : IEnumerable<T>
        {
            private readonly int _chunkSize;
            private readonly int _start;

            private readonly EnumeratorWrapper<T> _wrapper;

            public ChunkedEnumerable(EnumeratorWrapper<T> wrapper, int chunkSize, int start)
            {
                _wrapper = wrapper;
                _chunkSize = chunkSize;
                _start = start;
            }

            public IEnumerator<T> GetEnumerator()
                => new ChildEnumerator(this);

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            private class ChildEnumerator : IEnumerator<T>
            {
                private readonly ChunkedEnumerable<T> _parent;
                private T _current;
                private bool _done;
                private int _position;


                public ChildEnumerator(ChunkedEnumerable<T> parent)
                {
                    _parent = parent;
                    _position = -1;
                    parent._wrapper.AddRef();
                }

                public T Current
                {
                    get
                    {
                        if (_position == -1 || _done) throw new InvalidOperationException();
                        return _current;
                    }
                }

                public void Dispose()
                {
                    if (!_done)
                    {
                        _done = true;
                        _parent._wrapper.RemoveRef();
                    }
                }

                object IEnumerator.Current
                    => Current;

                public bool MoveNext()
                {
                    _position++;

                    if (_position + 1 > _parent._chunkSize) _done = true;

                    if (!_done) _done = !_parent._wrapper.Get(_position + _parent._start, out _current);

                    return !_done;
                }

                public void Reset()
                {
                    // per http://msdn.microsoft.com/en-us/library/system.collections.ienumerator.reset.aspx
                    throw new NotSupportedException();
                }
            }
        }

        private class EnumeratorWrapper<T>
        {
            private Enumeration _currentEnumeration;

            private int _refs;
            private IEnumerable<T> SourceEnumerable { get; }

            public EnumeratorWrapper(IEnumerable<T> source)
                => SourceEnumerable = source;

            public bool Get(int pos, out T item)
            {
                if (_currentEnumeration != null && _currentEnumeration.Position > pos)
                {
                    _currentEnumeration.Source.Dispose();
                    _currentEnumeration = null;
                }

                if (_currentEnumeration == null)
                    _currentEnumeration = new Enumeration
                        {Position = -1, Source = SourceEnumerable.GetEnumerator(), AtEnd = false};

                item = default(T);
                if (_currentEnumeration.AtEnd) return false;

                while (_currentEnumeration.Position < pos)
                {
                    _currentEnumeration.AtEnd = !_currentEnumeration.Source.MoveNext();
                    _currentEnumeration.Position++;

                    if (_currentEnumeration.AtEnd) return false;
                }

                item = _currentEnumeration.Source.Current;

                return true;
            }

            // needed for dispose semantics 
            public void AddRef()
            {
                _refs++;
            }

            public void RemoveRef()
            {
                _refs--;
                if (_refs == 0 && _currentEnumeration != null)
                {
                    var copy = _currentEnumeration;
                    _currentEnumeration = null;
                    copy.Source.Dispose();
                }
            }

            private class Enumeration
            {
                public IEnumerator<T> Source { get; set; }
                public int Position { get; set; }
                public bool AtEnd { get; set; }
            }
        }
    }
}