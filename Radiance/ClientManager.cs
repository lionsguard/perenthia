using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public class ClientManager : IList<IClient>
	{
		private List<IClient> _clients = new List<IClient>();

		public event EventHandler ClientAdded = delegate { };

		public IClient this[string key]
		{
			get 
			{
				lock (_clients)
				{
					return _clients.Where(c => c.UserName == key).FirstOrDefault();
				}
			}
		}

		public IClient this[Guid sessionId]
		{
			get
			{
				lock (_clients)
				{
					return _clients.Where(c => c.SessionId.Equals(sessionId)).FirstOrDefault();
				}
			}
		}

		public bool ContainsKey(string key)
		{
			lock (_clients)
			{
				return _clients.Where(c => c.UserName == key).Count() > 0;
			}
		}

		public bool ContainsKey(Guid sessionId)
		{
			lock (_clients)
			{
				return _clients.Where(c => c.SessionId.Equals(sessionId)).Count() > 0;
			}
		}

		public void Add(IClient item)
		{
			lock (_clients)
			{
				_clients.Add(item);
				this.ClientAdded(this, EventArgs.Empty);
			}
		}

		#region IList<IClient> Members

		public int IndexOf(IClient item)
		{
			lock (_clients)
			{
				return _clients.IndexOf(item);
			}
		}

		public void Insert(int index, IClient item)
		{
			lock (_clients)
			{
				_clients.Insert(index, item);
			}
		}

		public void RemoveAt(int index)
		{
			lock (_clients)
			{
				_clients.RemoveAt(index);
			}
		}

		public IClient this[int index]
		{
			get
			{
				lock (_clients)
				{
					return _clients[index];
				}
			}
			set
			{
				lock (_clients)
				{
					_clients[index] = value;
				}
			}
		}

		#endregion

		#region ICollection<IClient> Members


		public void Clear()
		{
			lock (_clients)
			{
				_clients.Clear();
			}
		}

		public bool Contains(IClient item)
		{
			lock (_clients)
			{
				return _clients.Contains(item);
			}
		}

		public void CopyTo(IClient[] array, int arrayIndex)
		{
			lock (_clients)
			{
				_clients.CopyTo(array, arrayIndex);
			}
		}

		public int Count
		{
			get
			{
				lock (_clients)
				{
					return _clients.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IClient item)
		{
			lock (_clients)
			{
				return _clients.Remove(item);
			}
		}

		#endregion

		#region IEnumerable<IClient> Members

		public IEnumerator<IClient> GetEnumerator()
		{
			lock (_clients)
			{
				return _clients.GetEnumerator();
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		public bool TryGetClient(string address, out IClient client)
		{
			lock (_clients)
			{
				for (int i = _clients.Count - 1; i >= 0; i--)
				{
					if (_clients[i].Address.Equals(address))
					{
						client = _clients[i];
						return true;
					}
				}
				client = null;
				return false;
			}
		}

		public bool TryGetClient(Guid sessionId, out IClient client)
		{
			lock (_clients)
			{
				for (int i = _clients.Count - 1; i >= 0; i--)
				{
					if (_clients[i].SessionId.Equals(sessionId))
					{
						client = _clients[i];
						return true;
					}
				}
				client = null;
				return false;
			}
		}
	}
}
