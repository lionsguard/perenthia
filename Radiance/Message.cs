using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// An enumeration of values indicating the network byte order of messages sent to and from the client and server.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// Specifies that a network byte order has not been defined.
		/// </summary>
		None,
		/// <summary>
		/// Specified network to host byte order.
		/// </summary>
		NetworkToHost,
		/// <summary>
		/// Specified host to network byte order.
		/// </summary>
		HostToNetwork,
	}

	/// <summary>
	/// Represents a message sent to and from the client and server.
	/// </summary>
	public class Message
	{
		/// <summary>
		/// Gets a default Message instance for invalid commands.
		/// </summary>
		public static readonly Message InvalidCommand = new Message(-1, "Invalid Command");

		/// <summary>
		/// Gets a default Message instance for errors that occur on the server.
		/// </summary>
		public static readonly Message Error = new Message(-2, "An internal error has occurred on the server. Your command was not executed, technical support has been notified of the issue.");

		/// <summary>
		/// Gets or sets a value indicating the type of command this message represents.
		/// </summary>
		public int Command { get; set; }

		/// <summary>
		/// Gets or sets the length of the message text.
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// The text or content of the message.
		/// </summary>
		public string Text
		{
			get 
			{
				if (this.HasData)
				{
					return Encoding.UTF8.GetString(this.Data, 0, this.Length);
				}
				return String.Empty;
			}
			set
			{
				this.Data = Encoding.UTF8.GetBytes(value);
			}
		}

		private byte[] _data = null;
		/// <summary>
		/// Gets or sets the data buffer for the current Message.
		/// </summary>
		public byte[] Data
		{
			get { return _data; }
			set
			{
				_data = value;
				if (_data != null && _data.Length > 0)
				{
					this.Length = _data.Length;
				}
				else
				{
					this.Length = 0;
				}
			}
		}

		public bool HasData
		{
			get { return (_data != null && _data.Length > 0); }
		}

		/// <summary>
		/// Gets or sets the authentication key string used to validate the executor of the message. Optional.
		/// </summary>
		public string AuthKey { get; set; }

		/// <summary>
		/// Gets or sets the length of the AuthKey string value.
		/// </summary>
		public int AuthKeyLength { get; set; }	

		/// <summary>
		/// Initializes a new instance of the Message class.
		/// </summary>
		/// <param name="command">The command this message represents.</param>
		/// <param name="text">The content of the message.</param>
		public Message(int command, string text)
			: this(command, text, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Message class.
		/// </summary>
		/// <param name="command">The command this message represents.</param>
		/// <param name="text">The content of the message.</param>
		/// <param name="authKey">The authentication string used to verify the current user.</param>
		public Message(int command, string text, string authKey)
			: this(command, Encoding.UTF8.GetBytes(text), authKey)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Message class.
		/// </summary>
		/// <param name="command">The command this message represents.</param>
		/// <param name="data">The data content byte array of the current message.</param>
		public Message(int command, byte[] data)
			: this(command, data, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Message class.
		/// </summary>
		/// <param name="command">The command this message represents.</param>
		/// <param name="data">The data content byte array of the current message.</param>
		/// <param name="authKey">The authentication string used to verify the current user.</param>
		public Message(int command, byte[] data, string authKey)
		{
			this.Command = command;
			this.Data = data;

			if (!String.IsNullOrEmpty(authKey))
			{
				this.AuthKey = authKey;
				this.AuthKeyLength = authKey.Length;
			}
		}

		/// <summary>
		/// Initializes a new instance of the Message class from the specified bytes.
		/// </summary>
		/// <param name="data">The byte array containing the complete Message instance.</param>
		public Message(byte[] data)
		{
			if (data != null)
			{
				// Command
				if (data.Length >= 4)
				{
					this.Command = BitConverter.ToInt32(data, 0);
				}
				// Data Length
				if (data.Length >= 8)
				{
					this.Length = BitConverter.ToInt32(data, 4);
				}
				// AuthKey Length
				if (data.Length >= 12)
				{
					this.AuthKeyLength = BitConverter.ToInt32(data, 8);
				}
				// Data
				if (this.Length > 0 && (data.Length - 12) >= this.Length)
				{
					_data = new byte[this.Length];
					Array.Copy(data, 12, _data, 0, this.Length);
				}
				// AuthKey
				if (this.AuthKeyLength > 0 && ((data.Length - 12) - this.Length) >= this.AuthKeyLength)
				{
					this.AuthKey = Encoding.UTF8.GetString(data, (this.Length + 12), this.AuthKeyLength);
				}
			}
		}

		/// <summary>
		/// Gets the current message as an array of bytes.
		/// </summary>
		/// <returns>An array of bytes.</returns>
		public byte[] GetBytes()
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(this.Command));
			list.AddRange(BitConverter.GetBytes(this.Length));
			list.AddRange(BitConverter.GetBytes(this.AuthKeyLength));
			if (this.HasData) list.AddRange(this.Data);
			if (!String.IsNullOrEmpty(this.AuthKey)) list.AddRange(Encoding.UTF8.GetBytes(this.AuthKey));
			return list.ToArray();
		}
	}

	/// <summary>
	/// Represents a collection of Message instances.
	/// </summary>
	public class MessageCollection : List<Message>
	{
		/// <summary>
		/// Gets a value used to lock the current object for synchronization.
		/// </summary>
		internal object SyncLock { get; private set; }	

		/// <summary>
		/// Initializes a new instance of the MessageCollection class.
		/// </summary>
		public MessageCollection()
		{
			this.SyncLock = new object();
		}

		/// <summary>
		/// Initializes a new instance of the MessageCollection class and adds message data to the collection.
		/// </summary>
		/// <param name="messages">The byte array of message data to parse into individual Message instances for the collection.</param>
		public MessageCollection(byte[] messages)
		{
			this.ParseBytes(messages);
		}

		private void ParseBytes(byte[] data)
		{
			Message msg = new Message(data);
			this.Add(msg);

			byte[] msgBytes = msg.GetBytes();
			int length = data.Length - msgBytes.Length;
			if (length >= 4)
			{
				byte[] buffer = new byte[length];
				Array.Copy(data, msgBytes.Length, buffer, 0, length);
				this.ParseBytes(buffer);
			}
		}

		/// <summary>
		/// Gets an array of bytes for the current MessageCollection instance.
		/// </summary>
		/// <returns>An array of bytes representing the current collection.</returns>
		public byte[] GetBytes()
		{
			List<byte> list = new List<byte>();
			foreach (var msg in this)	
			{
				list.AddRange(msg.GetBytes());
			}
			return list.ToArray();
		}
	}

	/// <summary>
	/// Represents a method for processing a Message.
	/// </summary>
	/// <param name="e">The MessageEventArgs containing the Message instance to process.</param>
	public delegate void MessageEventHandler(MessageEventArgs e);

	/// <summary>
	/// Provides a class for events that handle Message data.
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the Message instance reference for this event data.
		/// </summary>
		public Message Message { get; private set; }

		/// <summary>
		/// Initializes a new instance of the MessageEventArgs class.
		/// </summary>
		/// <param name="msg">The Message instance to include in the event.</param>
		public MessageEventArgs(Message msg)
		{
			this.Message = msg;
		}
	}
}
